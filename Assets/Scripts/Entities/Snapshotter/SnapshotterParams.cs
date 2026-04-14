using Character.Creator;
using Character.Data;

namespace Snapshotter
{
	public sealed class SnapshotterParams
	{

		public SnapshotterParams(SnapshotterCameraPosition camPos, ObservableCustomizationData data)
		{
			CamPos = camPos; ;
			Data = data;
		}

		public SnapshotterCameraPosition CamPos { get; }
		public ObservableCustomizationData Data { get; }

		/// <summary>
		/// Optional override for the pose the animator will play the first frame of
		/// </summary>
		public PoseId Pose { get; set; } = null;

		/// <summary>
		/// Optional override for a portrait that will be applied to the snapshot
		/// </summary>
		public PortraitId Portrait { get; set; } = null;
	}
}
