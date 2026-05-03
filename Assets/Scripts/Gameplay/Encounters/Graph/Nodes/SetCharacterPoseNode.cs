using Character.Data;
using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class SetCharacterPoseNode : SingleOutputNode
	{
		[field: SerializeField]
		public AssetReferenceT<PoseId> Pose { get; private set; }

		public SetCharacterPoseNode(PoseId pose)
		{
			Pose = new(pose.UniqueAssetID);
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.Data.PoseId = Pose.LoadSync();
			encounterInstance.ProgressToNode(_next);
		}
	}
}