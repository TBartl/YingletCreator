using Character.Creator;
using Snapshotter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YingSnapshotting;


/// <summary>
/// Provides a mechanism to get RenderTexture snapshots of characters based (currently) on their cached data
/// This has two benefits over doing it manually:
/// - It throttles requests, to not slow things down if there's a bunch of requests needed
/// - It shares render textures if multiple things are using the same cached data
/// </summary>
public interface IYingSnapshotManager
{
	/// <summary>
	/// Gets a render texture
	/// This will be shared by other sources trying to get a snapshot for the same cached data
	/// This should be disposed to ensure the render texture is cleaned up when no longer needed
	/// </summary>
	IYingSnapshotRenderTexture GetDataRenderTexture(SerializableCustomizationData yingletData);

	/// <summary>
	/// Alternate method to get a render texture
	/// This is also shared
	/// The difference is that this snapshot is taken from a character in the scene
	/// </summary>
	IYingSnapshotRenderTexture GetCharacterRenderTexture(ICharacterRoot characterRoot);

	ISnapshotterReferences References { get; }
	SnapshotterCameraPosition CameraPosition { get; }
}

interface IYingSnapshotManagerReferences
{
	ISnapshotterReferences References { get; }
	SnapshotterCameraPosition CameraPosition { get; }
	ICompositeResourceLoader ResourceLoader { get; }
	Coroutine StartCoroutine(IEnumerator routine);
}

public interface IYingSnapshotRenderTexture : IDisposable
{
	RenderTexture RenderTexture { get; }
}


public class YingSnapshotManager : MonoBehaviour, IYingSnapshotManager, IYingSnapshotManagerReferences
{
	[SerializeField] SnapshotterReferences _references;
	[SerializeField] AssetReferenceT<SnapshotterCameraPosition> _cameraPositionReference;

	Dictionary<SerializableCustomizationData, DataSnapshotDictValue> _dataSnapshots = new();
	Dictionary<ICharacterRoot, CharacterSnapshotDictValue> _characterSnapshots = new();
	private ICompositeResourceLoader _resourceLoader;

	public ISnapshotterReferences References => _references;
	public SnapshotterCameraPosition CameraPosition => _cameraPositionReference.LoadSync();
	public ICompositeResourceLoader ResourceLoader => _resourceLoader;

	private void Awake()
	{
		_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
	}

	public IYingSnapshotRenderTexture GetDataRenderTexture(SerializableCustomizationData customizationData)
	{
		if (customizationData == null) return null;

		DataSnapshotDictValue dictValue = null;
		if (_dataSnapshots.TryGetValue(customizationData, out var cachedDictValue))
		{
			dictValue = cachedDictValue;
		}
		if (dictValue == null)
		{
			dictValue = new DataSnapshotDictValue(this, customizationData);
			_dataSnapshots.Add(customizationData, dictValue);
		}

		dictValue.Watchers++;

		return new YingSnapshotRenderTexture(dictValue.RenderTexture, () =>
		{
			dictValue.Watchers--;
			if (dictValue.Watchers <= 0)
			{
				dictValue.Dispose();
				_dataSnapshots.Remove(customizationData);
			}
		});
	}

	public IYingSnapshotRenderTexture GetCharacterRenderTexture(ICharacterRoot characterRoot)
	{
		if (characterRoot == null) return null;

		CharacterSnapshotDictValue dictValue = null;
		if (_characterSnapshots.TryGetValue(characterRoot, out var cachedDictValue))
		{
			dictValue = cachedDictValue;
		}
		if (dictValue == null)
		{
			dictValue = new CharacterSnapshotDictValue(this, characterRoot);
			_characterSnapshots.Add(characterRoot, dictValue);
		}

		dictValue.Watchers++;

		return new YingSnapshotRenderTexture(dictValue.RenderTexture, () =>
		{
			dictValue.Watchers--;
			if (dictValue.Watchers <= 0)
			{
				dictValue.Dispose();
				_characterSnapshots.Remove(characterRoot);
			}
		});
	}
}
