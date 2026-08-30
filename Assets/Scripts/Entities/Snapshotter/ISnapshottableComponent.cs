using System;

namespace Snapshotter
{
	public enum SnapshotOrder
	{
		BeforeAnimate,
		Animate,
		AfterAnimate,
		CopyRig,
		ApplyBones
	}

	/// <summary>
	/// With the snapshotter, we spawn up and immediately remove an object
	/// This means that operations like applying bones which happen per-frame won't be setup
	/// This gives us a hook to run those operations at any time
	/// </summary>
	public interface ISnapshottableComponent
	{
		/// <summary>
		/// Prepares the component for snapshotting, returning a nullable action that will be called after the snapshot is taken to clean up any state
		/// </summary>
		Action PrepareForSnapshot(ISnapshotterReferences references);

		/// <summary>
		/// 0 is default. Anything less will run earlier
		/// </summary>
		SnapshotOrder SnapshotOrder { get; }
	}
}