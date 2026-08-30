using Snapshotter;
using System;
using System.Collections.Generic;
using UnityEngine;

internal interface IApplyableCustomization
{
	void Apply();
	bool isActiveAndEnabled { get; }
}

/// <summary>
/// We want to apply all of these options in order, as some affect others
/// </summary>
public class ApplyCustomizationsAfterAnimate : MonoBehaviour, ISnapshottableComponent
{
	private IEnumerable<IApplyableCustomization> _applyable;


	private void Awake()
	{
		_applyable = this.GetComponentsInChildren<IApplyableCustomization>();
	}

	// Do this every frame in late-update so it happens after the animator
	private void LateUpdate()
	{
		foreach (var a in _applyable)
		{
			if (!a.isActiveAndEnabled) continue;
			a.Apply();
		}
	}

	public SnapshotOrder SnapshotOrder => SnapshotOrder.ApplyBones;
	public Action PrepareForSnapshot(ISnapshotterReferences references)
	{
		LateUpdate();
		return null;
	}
}
