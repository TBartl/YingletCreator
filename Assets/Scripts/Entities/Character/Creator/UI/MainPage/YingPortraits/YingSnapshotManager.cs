using Character.Creator;
using Reactivity;
using Snapshotter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Responsible for
/// </summary>
public interface IYingSnapshotManager
{
	/// <summary>
	/// Gets a render texture that will be updated when possible
	/// This will be shared by other sources trying to get the same texture
	/// This should be disposed to ensure the render texture is cleaned up when no longer needed
	/// </summary>
	IYingSnapshotRenderTexture GetRenderTexture(ICachedYingletReference yingletData);

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

	Dictionary<ICachedYingletReference, DictValue> _snapshots = new();
	private ICompositeResourceLoader _resourceLoader;

	public ISnapshotterReferences References => _references;
	public SnapshotterCameraPosition CameraPosition => _cameraPositionReference.LoadSync();
	public ICompositeResourceLoader ResourceLoader => _resourceLoader;

	private void Awake()
	{
		_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
	}

	public IYingSnapshotRenderTexture GetRenderTexture(ICachedYingletReference yingletData)
	{
		DictValue dictValue = null;
		if (_snapshots.TryGetValue(yingletData, out var cachedDictValue))
		{
			dictValue = cachedDictValue;
		}
		if (dictValue == null)
		{
			dictValue = new DictValue(this, yingletData);
			_snapshots.Add(yingletData, dictValue);
		}

		dictValue.Watchers++;

		return new YingSnapshotRenderTexture(dictValue.RenderTexture, () =>
		{
			dictValue.Watchers--;
			if (dictValue.Watchers <= 0)
			{
				dictValue.Dispose();
				_snapshots.Remove(yingletData);
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
		ICachedYingletReference _yingReference;
		public RenderTexture RenderTexture { get; private set; }
		Reflector _reflector;

		public DictValue(IYingSnapshotManagerReferences snapshotReferences, ICachedYingletReference yingReference)
		{
			_snapshotReferences = snapshotReferences;
			_yingReference = yingReference;
			RenderTexture = SnapshotterUtils.CreateRenderTexture(snapshotReferences.References);
			_reflector = new Reflector(Reflect);
		}

		public void Dispose()
		{
			RenderTexture = null;
			_reflector.Destroy();
		}

		private void Reflect()
		{
			var cachedData = _yingReference.CachedData; // Not actually used; just for reflection
			RunThrottled(Snapshot);
		}

		void Snapshot()
		{
			var cachedData = _yingReference.CachedData;
			if (cachedData == null)
			{
				// Make the render texture transparent
				var previous = RenderTexture.active;
				RenderTexture.active = RenderTexture;
				GL.Clear(true, true, Color.clear);
				RenderTexture.active = previous;
				return;

			}
			var observableData = new ObservableCustomizationData(cachedData, _snapshotReferences.ResourceLoader);
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
