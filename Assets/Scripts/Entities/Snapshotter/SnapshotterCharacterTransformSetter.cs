using Snapshotter;
using System;
using UnityEngine;

public class SnapshotterCharacterTransformSetter : MonoBehaviour, ISnapshottableComponent
{
	public SnapshotOrder SnapshotOrder => SnapshotOrder.AfterAnimate;

	public Action PrepareForSnapshot(ISnapshotterReferences references)
	{
		var originalPos = transform.position;
		var originalRot = transform.rotation;

		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;

		return () =>
		{
			this.transform.position = originalPos;
			this.transform.rotation = originalRot;
		};
	}
}
