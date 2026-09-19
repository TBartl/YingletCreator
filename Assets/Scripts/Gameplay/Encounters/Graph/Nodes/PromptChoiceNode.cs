using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class PromptChoiceNode : IEncounterNode
	{
		[field: SerializeField]
		public ChoiceBlockNode[] Choices { get; private set; }

		public bool Blocking => true;

		public void EditorSetConnections(IList<IEncounterNode> connections)
		{
			Choices = connections.Cast<ChoiceBlockNode>().ToArray();
		}

		public void Run(IEncounterInstance encounterInstance)
		{
			// UI will call into our VisitData to progress
		}

		public IEncounterVisitData GenerateVisitData(IEncounterInstance encounterInstance)
		{
			return new PromptChoiceNodeVisitData(encounterInstance, this);
		}
	}

	sealed class PromptChoiceNodeVisitData : IEncounterVisitData, IDisposable
	{
		private IEncounterInstance _encounter;
		private PromptChoiceNode _node;
		private ulong _netId;

		public PromptChoiceNodeVisitData(IEncounterInstance encounter, PromptChoiceNode node)
		{
			_encounter = encounter;
			_node = node;
			_netId = encounter.Networking.IdentityProvider.GetNextId();
			_encounter.Networking.EventBus.Subscribe<Message_EncounterSelectChoice>(OnMessage_SelectChoice);
		}

		public void Dispose()
		{
			_encounter.Networking.EventBus.Unsubscribe<Message_EncounterSelectChoice>(OnMessage_SelectChoice);
		}


		public void SendMessage_SelectChoice(int choiceIndex)
		{
			_encounter.Networking.EventBus.SendToAll(new Message_EncounterSelectChoice(_netId, choiceIndex));
		}

		bool _ran = false;
		private void OnMessage_SelectChoice(Message_EncounterSelectChoice message, ulong senderClientId)
		{
			if (message.NetId != _netId) return;
			if (_ran) return;
			_ran = true;

			if (message.ChoiceIndex < 0 || message.ChoiceIndex >= _node.Choices.Length)
			{
				Debug.LogError($"Received choice index {message.ChoiceIndex} is out of range for current PromptChoiceNode.");
				return;
			}
			var choice = _node.Choices[message.ChoiceIndex];
			_encounter.ProgressToNode(choice);
		}
	}
}

public struct Message_EncounterSelectChoice : INetMessage
{
	public ulong NetId;
	public int ChoiceIndex;
	public Message_EncounterSelectChoice(ulong netId, int choiceIndex)
	{
		NetId = netId;
		ChoiceIndex = choiceIndex;
	}
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
		serializer.SerializeValue(ref ChoiceIndex);
	}
}