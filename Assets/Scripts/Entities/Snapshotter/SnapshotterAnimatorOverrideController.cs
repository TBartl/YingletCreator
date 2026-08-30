using System.Collections.Generic;
using UnityEngine;

public interface ISnapshotterAnimatorOverrideController
{
	void SetSnapshotterClip(AnimationClip clip);
}

internal class SnapshotterAnimatorOverrideController : MonoBehaviour, ISnapshotterAnimatorOverrideController
{
	public void SetSnapshotterClip(AnimationClip clip)
	{
		var animator = this.GetComponentInChildrenSafe<Animator>();
		var originalController = animator.runtimeAnimatorController;
		var overrideController = new AnimatorOverrideController(originalController);
		animator.runtimeAnimatorController = overrideController;
		var originalClip = overrideController.animationClips[0];
		overrideController.ApplyOverrides(new List<KeyValuePair<AnimationClip, AnimationClip>>() { new(originalClip, clip) });
	}
}
