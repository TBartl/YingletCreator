using Character.Data;
using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class SetCharacterPoseNode : SingleOutputNode
	{
		[field: SerializeField]
		public AssetReferenceT<PoseId> Pose { get; private set; }

		[field: SerializeField]
		public bool Mirror { get; private set; }

		public SetCharacterPoseNode(PoseId pose, bool mirror)
		{
			Pose = new(pose.UniqueAssetID);
			Mirror = mirror;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.Data.PoseId = Pose.LoadSync();
			encounterInstance.Data.Mirror = Mirror;
			encounterInstance.ProgressToNode(_next);
		}
	}
}