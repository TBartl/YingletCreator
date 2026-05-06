using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Encounters
{
	public enum RollType
	{
		Any,
		Body,
		Mind
	}
}

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class RollNode : IEncounterNode
	{
		[field: SerializeField] public RollType RollType { get; private set; }
		[field: SerializeField] public RollBlockNode[] Branches { get; private set; }

		public RollNode(RollType rollType)
		{
			RollType = rollType;
		}

		public void EditorSetConnections(IList<IEncounterNode> connections)
		{
			Branches = connections.Cast<RollBlockNode>().ToArray();
		}

		public bool Blocking => false;

		public void Run(IEncounterInstance encounterInstance)
		{
			var rollProvider = encounterInstance.EncounterSource.GetComponentInParentSafe<IRollProvider>();
			int rollResult = rollProvider.GetRoll(encounterInstance.Character, RollType);

			// Write this to the instance so the UI can read it
			encounterInstance.NodeResultData.Add(rollResult);

			var branch = Branches.FirstOrDefault(branch => rollResult <= branch.MaxValueInclusive);
			encounterInstance.ProgressToNode(branch); // Ok to be null
		}
	}
}
