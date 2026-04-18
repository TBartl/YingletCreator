using UnityEngine;

namespace Snapshotter
{
	public class AnimatorForceForSnapshot : MonoBehaviour, ISnapshottableComponent
	{
		public SnapshotOrder SnapshotOrder => SnapshotOrder.Animate;
		public void PrepareForSnapshot()
		{
			this.GetComponent<Animator>().Update(0.2f);
		}

	}
}
