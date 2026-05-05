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

		public void Run(IEncounterInstance encounterInstance)
		{
			// TODO: once we figure out how this will all work
			//var rng = encounterInstance.EncounterSource.GetComponentInParentSafe<IDeterministicRandomProvider>();
			//rng.GetNextRandomInt()

			var branch = Branches.FirstOrDefault();
			if (branch == null)
			{
				Debug.LogWarning($"No branches on {nameof(RollNode)}.");
			}
			encounterInstance.ProgressToNode(branch); // Ok to be null
		}
	}
}
