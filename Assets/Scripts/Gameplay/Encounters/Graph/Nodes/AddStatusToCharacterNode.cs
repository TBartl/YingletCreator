using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class AddStatusToCharacterNode : SingleOutputNode
	{
		[SerializeField] private AssetReferenceT<StatusId> _status;

		public StatusId Status => _status.LoadSync();

		public AddStatusToCharacterNode(StatusId status)
		{
			_status = new AssetReferenceT<StatusId>(status.UniqueAssetID);
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			var characterStatuses = encounterInstance.Character.GetComponentInChildrenSafe<ICharacterStatuses>();
			characterStatuses.AddStatus(Status);

			encounterInstance.ProgressToNode(_next);
		}
	}
}