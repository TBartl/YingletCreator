using Encounters.Runtime;
using Reactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
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
			return new RollNodeVisitData(encounterInstance, this);
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
		/// The roll finished - stay on it for a moment for the user
		/// </summary>
		ShowingResult,

		/// <summary>
		/// This roll has wrapped up and we've moved on
		/// </summary>
		Finished
	}

	sealed class RollNodeVisitData : IEncounterVisitData, IDisposable
	{
		const float TIME_TO_ROLL = 1f;
		const float TIME_TO_SHOW_RESULT = .6f;

		Computed<int> _statValue; // It's unlikely that anything changes this between when the encounter starts and when the roll is done, but just in case
		Computed<int> _expectedSum;

		Observable<int> _realSum = new Observable<int>(0);

		Observable<int[]> _diceRolls = new Observable<int[]>(null);

		Observable<RollState> _state = new Observable<RollState>(RollState.Prepare);
		private IEncounterInstance _encounter;
		private RollNode _node;
		private ulong _netId;

		public int StatValue => _statValue.Val;
		public int ExpectedSum => _expectedSum.Val;
		public int RealSum => _realSum.Val;
		public int[] DiceRolls => _diceRolls.Val;
		public RollState State => _state.Val;
		public IReadOnlyObservable<RollState> StateObservable => _state;
		public RollBlockNode ExpectedBranch => _node.Branches.GetBranch(ExpectedSum);
		public RollBlockNode RealBranch => _node.Branches.GetBranch(RealSum);

		public RollNodeVisitData(IEncounterInstance encounter, RollNode rollNode)
		{
			_encounter = encounter;
			_node = rollNode;
			_netId = encounter.Networking.IdentityProvider.GetNextId();

			_statValue = new Computed<int>(ComputeStat);
			_expectedSum = new Computed<int>(ComputeExpectedSum);

			_encounter.Networking.EventBus.Subscribe<Message_EncounterRoll>(OnMessage);
		}

		public void Dispose()
		{
			_statValue.Destroy();
			_encounter.Networking.EventBus.Unsubscribe<Message_EncounterRoll>(OnMessage);
		}

		private int ComputeStat()
		{
			var stat = _node.RollInstructions.Stat;
			if (stat == null) return 0;
			return _encounter.Character.GetComponentInChildrenSafe<ICharacterStats>().GetStat(stat);
		}

		private int ComputeExpectedSum()
		{
			return 7 + _statValue.Val;
		}

		public void SendMessage_Roll(RollState newState)
		{
			_encounter.Networking.EventBus.SendToAll(new Message_EncounterRoll(_netId, newState));
		}

		private void OnMessage(Message_EncounterRoll message, ulong senderClientId)
		{
			if (message.NetId != _netId) return;

			var newState = message.NewState;

			// Ensure we're coming from the previous state
			if (_state.Val != (newState - 1)) return;

			bool isClient = _encounter.Networking.NetState.IsClient();
			if (newState == RollState.Rolling)
			{
				// Calculate it here so the roll can show it a little early
				_realSum.Val = CalculateResult();
				if (!isClient)
				{
					CoroutineRunner.S.StartCoroutine(MoveToStateAfterTime(RollState.ShowingResult, TIME_TO_ROLL));
				}
			}
			else if (newState == RollState.ShowingResult && !isClient)
			{
				if (!isClient)
				{
					CoroutineRunner.S.StartCoroutine(MoveToStateAfterTime(RollState.Finished, TIME_TO_SHOW_RESULT));
				}
			}
			else if (newState == RollState.Finished)
			{
				_encounter.ProgressToNode(RealBranch);
			}

			_state.Val = newState;
		}

		int CalculateResult()
		{
			var rollProvider = _encounter.EncounterSource.GetComponentInParentSafe<IRollProvider>();

			int dice1 = rollProvider.RollD6();
			int dice2 = rollProvider.RollD6();
			_diceRolls.Val = new int[] { dice1, dice2 };

			if (dice1 == dice2)
			{
				if (dice1 == 1)
				{
					// Double ones, always fail
					return 0;
				}
				else if (dice1 == 6)
				{
					// Double sixes, always succeed
					return RollProvider.MaxRollValue;
				}
			}

			// Eventually add modifiers here
			return dice1 + dice2 + _statValue.Val;
		}

		IEnumerator MoveToStateAfterTime(RollState newState, float time)
		{
			yield return new WaitForSeconds(time);
			SendMessage_Roll(newState);
		}
	}

	public static class RollNodeExtensionMethods
	{
		public static RollBlockNode GetBranch(this RollBlockNode[] branches, int rollResult)
		{
			rollResult = Mathf.Min(rollResult, 999); // Just in case someone mis-inputted the number
			RollBlockNode bestBranch = null;
			foreach (var branch in branches.Reverse())
			{
				if (rollResult > branch.MaxValueInclusive) break;
				bestBranch = branch;
			}
			if (bestBranch == null)
			{
				Debug.LogError($"No branch found for roll result {rollResult}");
			}
			return bestBranch;
		}
	}
}

public struct Message_EncounterRoll : INetMessage
{
	public ulong NetId;
	public RollState NewState;

	public Message_EncounterRoll(ulong netId, RollState newState)
	{
		NetId = netId;
		NewState = newState;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
		serializer.SerializeValue(ref NewState);
	}
}
