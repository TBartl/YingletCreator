using System;
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

		public string RollInstructionsName => RollInstructions.Stat?.DisplayName?.ToUpper() ?? "Luck";

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
			//var rollProvider = encounterInstance.EncounterSource.GetComponentInParentSafe<IRollProvider>();
			//int rollResult = rollProvider.GetRoll(encounterInstance.Character, RollInstructions);

			//// Write this to the instance so the UI can read it
			//encounterInstance.NodeResultData.Add(rollResult);

			//var branch = Branches.FirstOrDefault(branch => rollResult <= branch.MaxValueInclusive);
			//encounterInstance.ProgressToNode(branch); // Ok to be null




			// TTODO
			//else if (node is RollBlockNode rollBlockNode)
			//{
			//	// We create the UI when the block has been selected since that's when all the data is available
			//	// Figure out the note that originated it
			//	var rollNode = (RollNode)(encounter.NodeHistory[indexInHistory - 1]);
			//	GameObject rollObject = Instantiate(_rollPrefab, transform);
			//	SetReferenceUI(rollObject);
			//	rollObject.GetComponentInChildrenSafe<IRollUI>().SetNode(encounter, rollNode, rollBlockNode, GetNextData(encounter));
			//	_positioner.ObjectAdded(false);
			//}
		}

		public IEncounterVisitData GenerateVisitData(IEncounterInstance encounterInstance)
		{
			// TTODO
			return null;
		}
	}

	public enum RollState
	{
		/// <summary>
		/// Dialogue initially shown
		/// </summary>
		Prepare,

		/// <summary>
		/// User hit a button to roll and the roll is being animated
		/// </summary>
		Rolling,

		/// <summary>
		/// Roll occurred and this is no longer shown
		/// </summary>
		Finished
	}

	sealed class RollNodeVisitData : IEncounterVisitData, IDisposable
	{

		private ulong _netId;
		public RollNodeVisitData(IEncounterInstance encounter, RollNode rollNode)
		{
			_netId = encounter.Networking.IdentityProvider.GetNextId();
		}

		public void Dispose()
		{
		}
	}
}
