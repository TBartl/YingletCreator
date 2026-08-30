using Character.Creator;
using Snapshotter;
using System;
using System.Collections;
using UnityEngine;

namespace YingSnapshotting
{
	/// <summary>
	/// Base class that maintains the render texture for character snapshots
	/// </summary>
	abstract class SnapshotDictValue : IDisposable
	{
		protected IYingSnapshotManagerReferences _snapshotReferences;

		public int Watchers { get; set; } = 0;
		public RenderTexture RenderTexture { get; protected set; }

		protected SnapshotDictValue(IYingSnapshotManagerReferences snapshotReferences)
		{
			_snapshotReferences = snapshotReferences;
			RenderTexture = SnapshotterUtils.CreateRenderTexture(snapshotReferences.References);
			RunThrottled(Snapshot);
		}

		public void Dispose()
		{
			RenderTexture.Release();
			RenderTexture = null;
		}

		protected abstract void Snapshot();

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

	/// <summary>
	/// SnapshotDictValue variant for SerializableCustomizationData
	/// </summary>
	sealed class DataSnapshotDictValue : SnapshotDictValue
	{
		SerializableCustomizationData _customizationData;

		public DataSnapshotDictValue(IYingSnapshotManagerReferences snapshotReferences, SerializableCustomizationData customizationData)
			: base(snapshotReferences)
		{
			_customizationData = customizationData;
		}

		protected override void Snapshot()
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
	}

	/// <summary>
	/// SnapshotDictValue variant for ICharacterRoot
	/// </summary>
	sealed class CharacterSnapshotDictValue : SnapshotDictValue
	{
		ICharacterRoot _characterRoot;

		public CharacterSnapshotDictValue(IYingSnapshotManagerReferences snapshotReferences, ICharacterRoot characterRoot)
			: base(snapshotReferences)
		{
			_characterRoot = characterRoot;
		}

		protected override void Snapshot()
		{
			var parameters = new SnapshotterParams(_snapshotReferences.CameraPosition, _characterRoot);

			// Don't need to apply portrait - ISnapshottableComponents will pull it

			RenderTexture = SnapshotterUtils.Snapshot(
				_snapshotReferences.References,
				parameters,
				RenderTexture);
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
