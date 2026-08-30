using Character.Creator;
using Snapshotter;
using System;
using UnityEngine;

internal class SnapshotterSetToDefaultPortraitPose : MonoBehaviour, ISnapshottableComponent, IInitializable
{
	private IYingletAnimationBridge _animBridge;
	private ICustomizationDataRepository _dataRepo;

	public SnapshotOrder SnapshotOrder => SnapshotOrder.BeforeAnimate;

	public void Initialize()
	{
		_animBridge = this.GetComponentSafe<IYingletAnimationBridge>();
		_dataRepo = this.GetComponentInParentSafe<ICustomizationDataRepository>();

	}

	public Action PrepareForSnapshot(ISnapshotterReferences references)
	{
		var pose = _dataRepo.CustomizationData.PortraitData.PortraitId.Val?.Pose;
		if (pose != null)
		{
			_animBridge.SetSnapshotterClip(pose);
		}
		return () =>
		{
			_animBridge.SetSnapshotterClip(null);
		};
	}
}
