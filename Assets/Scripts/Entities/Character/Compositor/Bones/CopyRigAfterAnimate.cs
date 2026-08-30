using Snapshotter;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I originally tried to have just a single rig
/// However, I found it increasingly difficult to apply all the slider values properly
/// - Unity doesn't always write the location / rotation / scale if it's not changing, so if it's written to as part of the rig update we're sunk
/// - We can't really detach things cleanly like we'd like to
/// So I've decided to make a second rig. This script will force it to match the first rig right after the update
/// This will allow me to recklessly apply whatever I want to this rig with the reassurance that it will all be fresh the next frame
/// </summary>
public class CopyRigAfterAnimate : MonoBehaviour, ISnapshottableComponent
{
	[SerializeField] Transform _sourceRoot;
	[SerializeField] Transform _sourceTransform;
	private CopyBone[] _copyBonesArray;

	private void Awake()
	{
		SetupCopyBones();
		Application.quitting += Application_quitting;
	}
	void OnDestroy()
	{
		Application.quitting -= Application_quitting;
	}

	private void Application_quitting()
	{
		LateUpdate(); // If we don't do this, jiggle bones will complain because we've broken the references on some bones :/
		Application.quitting -= Application_quitting;
	}

	private void SetupCopyBones()
	{
		var copyBonesList = new List<CopyBone>();
		RecursiveSetup(_sourceRoot, _sourceTransform);

		_copyBonesArray = copyBonesList.ToArray();

		void RecursiveSetup(Transform source, Transform target)
		{
			// Build a lookup table for the target transform
			var targetChildren = new Dictionary<string, Transform>();
			for (int i = 0; i < target.childCount; i++)
			{
				Transform targetChild = target.GetChild(i);
				targetChildren[targetChild.name] = targetChild;
			}

			// Only add matching names. Other non-bone children may have been added for things like head tracking
			for (int i = 0; i < source.childCount; i++)
			{
				Transform sourceChild = source.GetChild(i);

				if (targetChildren.TryGetValue(sourceChild.name, out Transform targetChild))
				{
					copyBonesList.Add(new CopyBone(sourceChild, targetChild));
					RecursiveSetup(sourceChild, targetChild);
				}
			}
		}
	}


	private void LateUpdate()
	{
		CopyBones();
	}
	void CopyBones()
	{
		foreach (var copyBone in _copyBonesArray)
		{
			copyBone.Copy();
		}
	}
	public SnapshotOrder SnapshotOrder => SnapshotOrder.CopyRig;
	public Action PrepareForSnapshot(ISnapshotterReferences references)
	{
		CopyBones();
		return null;
	}

	sealed class CopyBone
	{
		private readonly Transform _source;
		private readonly Transform _target;
		private readonly Transform _parent;
		private readonly int _siblingIndex;

		public CopyBone(Transform source, Transform target)
		{
			_source = source;
			_target = target;
			_parent = target.parent;
			_siblingIndex = target.GetSiblingIndex();
		}
		public void Copy()
		{
			_target.SetParent(_parent, false);
			_target.SetSiblingIndex(_siblingIndex);
			_target.position = _source.position;
			_target.rotation = _source.rotation;
			_target.localScale = _source.localScale;
		}
	}
}
