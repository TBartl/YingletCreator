using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapshotter
{
	/// <summary>
	/// Creates 
	/// </summary>
	internal sealed class SnapshotterCharacterObjectHandler : IDisposable
	{
		private readonly ISnapshotterReferences _references;
		bool _existingObject = false;
		private readonly GameObject _yingletInstance;
		List<Action> _restoreActions = new List<Action>();

		public SnapshotterCharacterObjectHandler(ISnapshotterReferences references, SnapshotterParams sParams)
		{
			_references = references;

			if (sParams.Data == null && sParams.Character == null)
			{
				throw new ArgumentException("Either Data or Character must be provided in SnapshotterParams.");
			}

			if (sParams.Character != null)
			{
				_yingletInstance = sParams.Character.gameObject;
				_existingObject = true;
			}
			else
			{
				using (_references.YingletPrefab.TemporarilyDisable())
				{
					_yingletInstance = GameObject.Instantiate(_references.YingletPrefab);
					var dataRepo = _yingletInstance.GetComponentSafe<SnapshotterDataRepository>();
					dataRepo.Setup(sParams.Data);

					// Setting this pretty much exclusively for MeshGatherer_FromSnapshotData
					// Honestly, I should probably get rid of that and replace it with a more traditional toggle system now that I have that
					dataRepo.Pose = sParams.Pose;

					var animatorController = _yingletInstance.GetComponentInChildrenSafe<ISnapshotterAnimatorOverrideController>();
					if (sParams.Pose != null)
					{
						animatorController.SetSnapshotterClip(sParams.Pose.Clip);
					}
					if (sParams.Portrait != null)
					{
						animatorController.SetSnapshotterClip(sParams.Portrait.Pose);
					}
				}
			}

			_yingletInstance.SetActive(true);

			var snapshottables = _yingletInstance
				.GetComponentsInChildrenSafe<ISnapshottableComponent>()
				.OrderBy(s => s.SnapshotOrder)
				.ToArray();

			foreach (var snapshottable in snapshottables)
			{
				//Debug.Log($"Preparing for snapshot: {snapshottable.GetType().Name} with order {snapshottable.SnapshotOrder}");
				var restoreAction = snapshottable.PrepareForSnapshot(_references);
				if (restoreAction != null)
				{
					_restoreActions.Add(restoreAction);
				}
			}
		}

		public void Dispose()
		{
			_references.YingletPrefab.SetActive(true);

			if (_existingObject)
			{
				foreach (var restoreAction in _restoreActions)
				{
					restoreAction();
				}
			}
			else
			{
				if (_yingletInstance != null && _references.CleanupObjects)
				{
					GameObject.DestroyImmediate(_yingletInstance);
				}
			}
		}

		public float GetYScale()
		{
			return _yingletInstance.GetComponentInChildrenSafe<IYingletHeightProvider>().YScale;
		}
	}
}
