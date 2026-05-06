using Encounters.Runtime;
using Networking;
using System;
using Unity.Netcode;
using UnityEngine;

public interface IEncounterNetworking : IDisposable
{
	void SendMessage_Continue();
	void SendMessage_SelectChoice(int choiceIndex);
}

public class EncounterNetworking : IEncounterNetworking
{
	private readonly INetEventBus _eventBus;
	private readonly ulong _netId;
	private readonly IEncounterInstance _encounter;

	public EncounterNetworking(IEncounterInstance encounter)
	{
		var idProvider = Singletons.GetSingleton<INetIdentityProvider>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_encounter = encounter;
		_netId = idProvider.GetNextId();

		_eventBus.Subscribe<Message_EncounterContinue>(OnMessage_Continue);
		_eventBus.Subscribe<Message_EncounterSelectChoice>(OnMessage_SelectChoice);
	}

	public void Dispose()
	{
		_eventBus.Unsubscribe<Message_EncounterContinue>(OnMessage_Continue);
		_eventBus.Unsubscribe<Message_EncounterSelectChoice>(OnMessage_SelectChoice);
	}

	public void SendMessage_Continue()
	{
		_eventBus.SendToAll(new Message_EncounterContinue(_netId, _encounter.NodeHistory.Count));
	}

	private void OnMessage_Continue(Message_EncounterContinue message, ulong senderClientId)
	{
		if (message.EncounterNetId != _netId) return;
		if (message.HistoryIndex != _encounter.NodeHistory.Count) return; // User may have been spam clicking continue

		var currentNode = _encounter.CurrentNode.Val;
		var nodeAsContinueNode = currentNode as PromptContinueNode;
		if (nodeAsContinueNode == null)
		{
			Debug.LogError("PromptContinueButton was clicked, but the current node is not a PromptContinueNode.");
			return;
		}
		nodeAsContinueNode.Continue(_encounter);
	}

	public void SendMessage_SelectChoice(int choiceIndex)
	{
		_eventBus.SendToAll(new Message_EncounterSelectChoice(_netId, _encounter.NodeHistory.Count, choiceIndex));
	}

	private void OnMessage_SelectChoice(Message_EncounterSelectChoice message, ulong senderClientId)
	{
		if (message.EncounterNetId != _netId) return;
		if (message.HistoryIndex != _encounter.NodeHistory.Count) return; // User may have been spam clicking choices
		var currentNode = _encounter.CurrentNode.Val;
		var nodeAsPromptChoiceNode = currentNode as PromptChoiceNode;
		if (nodeAsPromptChoiceNode == null)
		{
			Debug.LogError("PromptChoiceUI was clicked, but the current node is not a PromptChoiceNode.");
			return;
		}
		if (message.ChoiceIndex < 0 || message.ChoiceIndex >= nodeAsPromptChoiceNode.Choices.Length)
		{
			Debug.LogError($"Received choice index {message.ChoiceIndex} is out of range for current PromptChoiceNode.");
			return;
		}
		var choice = nodeAsPromptChoiceNode.Choices[message.ChoiceIndex];
		_encounter.ProgressToNode(choice);
	}
}



public struct Message_EncounterContinue : INetMessage
{
	public ulong EncounterNetId;
	public int HistoryIndex;

	public Message_EncounterContinue(ulong encounterNetId, int historyIndex)
	{
		EncounterNetId = encounterNetId;
		HistoryIndex = historyIndex;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref EncounterNetId);
		serializer.SerializeValue(ref HistoryIndex);
	}
}

public struct Message_EncounterSelectChoice : INetMessage
{
	public ulong EncounterNetId;
	public int HistoryIndex;
	public int ChoiceIndex;
	public Message_EncounterSelectChoice(ulong encounterNetId, int historyIndex, int choiceIndex)
	{
		EncounterNetId = encounterNetId;
		HistoryIndex = historyIndex;
		ChoiceIndex = choiceIndex;
	}
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref EncounterNetId);
		serializer.SerializeValue(ref HistoryIndex);
		serializer.SerializeValue(ref ChoiceIndex);
	}
}