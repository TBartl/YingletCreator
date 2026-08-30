using System;
using UnityEngine;

namespace Snapshotter
{
	public class SnapshotterForcePlayAnimator : MonoBehaviour, ISnapshottableComponent, IInitializable
	{
		private Animator _animator;

		public SnapshotOrder SnapshotOrder => SnapshotOrder.Animate;

		public void Initialize()
		{
			_animator = this.GetComponentSafe<Animator>();
		}

		public Action PrepareForSnapshot(ISnapshotterReferences references)
		{
			_animator.enabled = false;
			_animator.enabled = true;
			_animator.Update(0.05f);
			return null;
		}

	}
}
