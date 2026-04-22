using Character.Creator;
using Snapshotter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
	IYingSnapshotRenderTexture GetRenderTexture(SerializableCustomizationData yingletData);

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

	Dictionary<SerializableCustomizationData, DictValue> _snapshots = new();
	private ICompositeResourceLoader _resourceLoader;

	public ISnapshotterReferences References => _references;
	public SnapshotterCameraPosition CameraPosition => _cameraPositionReference.LoadSync();
	public ICompositeResourceLoader ResourceLoader => _resourceLoader;

	private void Awake()
	{
		_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
	}

	public IYingSnapshotRenderTexture GetRenderTexture(SerializableCustomizationData customizationData)
	{
		if (customizationData == null)
		{
			return null;
		}

		DictValue dictValue = null;
		if (_snapshots.TryGetValue(customizationData, out var cachedDictValue))
		{
			dictValue = cachedDictValue;
		}
		if (dictValue == null)
		{
			dictValue = new DictValue(this, customizationData);
			_snapshots.Add(customizationData, dictValue);
		}

		dictValue.Watchers++;

		return new YingSnapshotRenderTexture(dictValue.RenderTexture, () =>
		{
			dictValue.Watchers--;
			if (dictValue.Watchers <= 0)
			{
				dictValue.Dispose();
				_snapshots.Remove(customizationData);
			}
		});
	}


	sealed class DictValue : IDisposable
	{
		// Should this take an observable instead? Or maybe a separate implementation of this that takes an observable baseline?
		// There's at least three use-cases:
		// - Customization portraits
		// - Expedition planning portraits
		// - Ingame portraits
		// - Ingame emotes(?)
		// Currently, this suffices for the first two


		IYingSnapshotManagerReferences _snapshotReferences;

		public int Watchers { get; set; } = 0;
		SerializableCustomizationData _customizationData;
		public RenderTexture RenderTexture { get; private set; }

		public DictValue(IYingSnapshotManagerReferences snapshotReferences, SerializableCustomizationData customizationData)
		{
			_snapshotReferences = snapshotReferences;
			_customizationData = customizationData;
			RenderTexture = SnapshotterUtils.CreateRenderTexture(snapshotReferences.References);
			RunThrottled(Snapshot);
		}

		public void Dispose()
		{
			RenderTexture.Release();
			RenderTexture = null;
		}

		void Snapshot()
		{
			var observableData = new ObservableCustomizationData(_customizationData, _snapshotReferences.ResourceLoader);
			var parameters = new SnapshotterParams(_snapshotReferences.CameraPosition, observableData);

			// Apply portrait if it exists
			var portrait = observableData.PortraitData.PortraitId.Val;
			if (portrait != null)
			{
				parameters.Portrait = portrait;
			}

			RenderTexture = SnapshotterUtils.Snapshot(
				_snapshotReferences.References,
				parameters,
				RenderTexture);
		}

		static Coroutine currentChain;
		void RunThrottled(Action action)
		{
			IEnumerator Chain()
			{
				// Wait until the current chain is done
				if (currentChain != null)
					yield return currentChain;

				yield return null;

				action();

				currentChain = null;
			}

			currentChain = _snapshotReferences.StartCoroutine(Chain());
		}

	}

	sealed class YingSnapshotRenderTexture : IYingSnapshotRenderTexture
	{
		private Action _dispose;
		public YingSnapshotRenderTexture(RenderTexture renderTexture, Action dispose)
		{
			RenderTexture = renderTexture;
			_dispose = dispose;
		}
		public RenderTexture RenderTexture { get; }

		public void Dispose()
		{
			_dispose();
		}
	}
}
