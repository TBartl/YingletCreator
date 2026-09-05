using Character.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapshotter
{
	internal sealed class SnapshotterPrefabHandler : IDisposable
	{
		private readonly ISnapshotterReferences _references;
		private readonly GameObject _yingletInstance;

		public SnapshotterPrefabHandler(ISnapshotterReferences references, SnapshotterParams sParams)
		{
			_references = references;

			if (sParams.Character == null && sParams.Data == null)
			{
				throw new ArgumentException("Either Character or Data must be provided");
			}

			var prefab = sParams.Character != null ? _references.YingletExpeditionPrefab : _references.YingletDataPrefab;
			PortraitId portrait;

			using (prefab.TemporarilyDisable())
			{
				_yingletInstance = GameObject.Instantiate(prefab);

				if (sParams.Character != null)
				{
					_yingletInstance.GetComponentSafe<ISnapshotterRelay>().RelayedCharacter = sParams.Character;
					portrait = sParams.Character.GetComponentInChildrenSafe<ICharacterPortraitProvider>().Portrait;
				}
				else
				{
					_yingletInstance.GetComponentSafe<SnapshotterDataRepository>().Setup(sParams.Data);
					portrait = sParams.Data.ToggleData.Toggles.GetLastComponentOrDefault<PortraitId>();
				}

				ApplyPoseIfPresent(_yingletInstance, sParams.Pose);
				ApplyPortraitIfPresent(_yingletInstance, portrait);

				_yingletInstance.SetActive(true);
			}
			var snapshottables = _yingletInstance
				.GetComponentsInChildren<ISnapshottableComponent>()
				.OrderBy(s => s.SnapshotOrder)
				.ToArray();
			foreach (var snapshottable in snapshottables)
			{
				snapshottable.PrepareForSnapshot();
			}
			SetLayerRecursively(_yingletInstance, _references.LayerIndex);
		}

		static void ApplyPoseIfPresent(GameObject yingletInstance, PoseId pose)
		{
			if (pose == null) return;
			ApplyClipIfPresent(yingletInstance, pose.Clip);

			// This is only really used for props
			// And those should probably be added with some other mechanism instead (like toggles)
			// Might try to remove this eventually
			yingletInstance.GetComponentInChildren<SnapshotterDataRepository>().Pose = pose;
		}

		static void ApplyClipIfPresent(GameObject yingletInstance, AnimationClip clip)
		{
			if (clip == null) return;
			var animator = yingletInstance.GetComponentInChildren<Animator>();
			var originalController = animator.runtimeAnimatorController;
			var overrideController = new AnimatorOverrideController(originalController);
			animator.runtimeAnimatorController = overrideController;
			var originalClip = overrideController.animationClips[0];
			overrideController.ApplyOverrides(new List<KeyValuePair<AnimationClip, AnimationClip>>() { new(originalClip, clip) });
		}
		static void ApplyPortraitIfPresent(GameObject yingletInstance, PortraitId portrait)
		{
			if (portrait == null) return;
			ApplyClipIfPresent(yingletInstance, portrait.Pose);
		}

		static void SetLayerRecursively(GameObject obj, int layer)
		{
			obj.layer = layer;

			foreach (Transform child in obj.transform)
			{
				SetLayerRecursively(child.gameObject, layer);
			}
		}

		public void Dispose()
		{
			_references.YingletDataPrefab.SetActive(true);
			_references.YingletExpeditionPrefab.SetActive(true);
			if (_yingletInstance != null && _references.CleanupObjects)
			{
				GameObject.DestroyImmediate(_yingletInstance);
			}
		}

		public float GetYScale()
		{
			return _yingletInstance.GetComponentInChildren<IYingletHeightProvider>().YScale;
		}
	}
}
