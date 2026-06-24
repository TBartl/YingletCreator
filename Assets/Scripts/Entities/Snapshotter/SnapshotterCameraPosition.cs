using UnityEngine;

namespace Snapshotter
{
	[CreateAssetMenu(fileName = "SnapshotterCameraPosition", menuName = "Scriptable Objects/Misc/SnapshotterCameraPosition")]
	public class SnapshotterCameraPosition : ScriptableObject
	{
		public Vector3 Position;
		public Vector3 Rotation;
	}
}
