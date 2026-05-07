using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Encounters.Runtime
{

	[System.Serializable]
	public sealed class RollInstructions
	{
		[SerializeField] AssetReferenceT<StatId> _stat;
		public StatId Stat => _stat?.LoadSync();
		public RollInstructions(StatId stat)
		{
			if (stat != null)
			{
				_stat = new AssetReferenceT<StatId>(stat.UniqueAssetID);
			}
		}
	}

	[System.Serializable]
	public sealed class RollNode : IEncounterNode
	{
		[field: SerializeField] public RollBlockNode[] Branches { get; private set; }
		[field: SerializeField] public RollInstructions RollInstructions { get; private set; }

		public string RollInstructionsName => RollInstructions.Stat.name?.ToString().ToUpper() ?? "Luck";

		public RollNode(StatId stat)
		{
			RollInstructions = new RollInstructions(stat);
		}

		public void EditorSetConnections(IList<IEncounterNode> connections)
		{
			Branches = connections.Cast<RollBlockNode>().ToArray();
		}

		public bool Blocking => false;

		public void Run(IEncounterInstance encounterInstance)
		{
			var rollProvider = encounterInstance.EncounterSource.GetComponentInParentSafe<IRollProvider>();
			int rollResult = rollProvider.GetRoll(encounterInstance.Character, RollInstructions);

			// Write this to the instance so the UI can read it
			encounterInstance.NodeResultData.Add(rollResult);

			var branch = Branches.FirstOrDefault(branch => rollResult <= branch.MaxValueInclusive);
			encounterInstance.ProgressToNode(branch); // Ok to be null
		}
	}
}
