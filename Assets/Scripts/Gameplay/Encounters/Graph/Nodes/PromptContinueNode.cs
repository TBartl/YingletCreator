using System;
using Unity.Netcode;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class PromptContinueNode : SingleOutputNode // For now; later we'll want to support multiple outputs for different choices
	{
		public override IEncounterVisitData GenerateVisitData(IEncounterInstance encounterInstance)
		{
			return new PromptContinueNodeVisitData(encounterInstance, this);
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			// UI will call into our VisitData to progress
		}

		public override bool Blocking => true;

		public void Continue(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
		}

	}

	sealed class PromptContinueNodeVisitData : IEncounterVisitData, IDisposable
	{
		private IEncounterInstance _encounter;
		private PromptContinueNode _node;
		private ulong _netId;

		public PromptContinueNodeVisitData(IEncounterInstance encounter, PromptContinueNode node)
		{
			_encounter = encounter;
			_node = node;
			_netId = encounter.Networking.IdentityProvider.GetNextId();
			_encounter.Networking.EventBus.Subscribe<Message_EncounterContinue>(OnMessage_Continue);
		}

		public void Dispose()
		{
			_encounter.Networking.EventBus.Unsubscribe<Message_EncounterContinue>(OnMessage_Continue);
		}

		public void SendMessage_Continue()
		{
			_encounter.Networking.EventBus.SendToAll(new Message_EncounterContinue(_netId));
		}

		bool _ran = false;
		private void OnMessage_Continue(Message_EncounterContinue message, ulong senderClientId)
		{
			if (message.NetId != _netId) return;
			if (_ran) return;

			_node.Continue(_encounter);
			_ran = true;
		}
	}
}

public struct Message_EncounterContinue : INetMessage
{
	public ulong NetId;

	public Message_EncounterContinue(ulong encounterNetId)
	{
		NetId = encounterNetId;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
	}
}