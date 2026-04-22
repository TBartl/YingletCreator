using Assets.Scripts.Entities.Character.Creator.Data;
using Character.Creator;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public sealed class ExpeditionPartyMember : INetworkSerializable
{
	public ExpeditionPartyMember(uint id, ulong clientId, SerializableCustomizationData customizationData)
	{
		Id = id;
		CustomizationData = customizationData;
		ClientId = clientId;
	}

	public uint Id;
	public ulong ClientId;
	public SerializableCustomizationData CustomizationData;


	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref Id);
		serializer.SerializeValue(ref ClientId);
		serializer.SerializeCustomizationData(ref CustomizationData);
	}
}

public interface IExpeditionPlanningManager
{
	IList<ExpeditionPartyMember> CurrentParty { get; }

	void AddToParty(SerializableCustomizationData customizationData);
	void RemoveFromParty(uint id);
}


public class ExpeditionPlanningManager : MonoBehaviour, IExpeditionPlanningManager
{
	public const int MAX_CHARACTERS = 4;
	uint _currentId = 0;

	private ObservableList<ExpeditionPartyMember> _currentParty = new ObservableList<ExpeditionPartyMember>();
	private INetStateReader _netState;
	private INetEventBus _netEventBus;
	private INetClientTracker _clientTracker;

	public IList<ExpeditionPartyMember> CurrentParty => _currentParty;

	private void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_netEventBus = Singletons.GetSingleton<INetEventBus>();
		_clientTracker = Singletons.GetSingleton<INetClientTracker>();

		_netState.OnLocalDisconnected += NetState_OnLocalDisconnected;
		_netEventBus.Subscribe<Message_AddExpeditionPartyMember>(OnAddExpeditionPartyMember);
		_netEventBus.Subscribe<Message_RemoveExpeditionPartyMember>(OnRemoveExpeditionPartyMember);
		_netEventBus.Subscribe<Message_InitializeExpeditionPartyForClient>(OnInitializeExpeditionPartyForClient);
		_clientTracker.OnClientConnectedToUs += ClientTracker_OnClientConnectedToUs;
		_clientTracker.OnClientDisconnectedFromUs += ClientTracker_OnClientDisconnectedFromUs;
	}

	private void OnDestroy()
	{
		_netState.OnLocalDisconnected -= NetState_OnLocalDisconnected;
	}

	private void NetState_OnLocalDisconnected()
	{
		_currentParty.Clear();
		_currentId = 0;
	}

	public void AddToParty(SerializableCustomizationData customizationData)
	{
		_netEventBus.SendToAll(new Message_AddExpeditionPartyMember { CustomizationData = customizationData });
	}

	public void RemoveFromParty(uint id)
	{
		_netEventBus.SendToAll(new Message_RemoveExpeditionPartyMember { Id = id });
	}

	private void OnAddExpeditionPartyMember(Message_AddExpeditionPartyMember message, ulong senderClientId)
	{

		if (_currentParty.Count < MAX_CHARACTERS)
		{
			var newMember = new ExpeditionPartyMember(_currentId++, senderClientId, message.CustomizationData);
			_currentParty.Add(newMember);
		}
		else
		{
			Debug.LogWarning("Max party size reached");
		}
	}

	private void OnRemoveExpeditionPartyMember(Message_RemoveExpeditionPartyMember message, ulong senderClientId)
	{
		var member = _currentParty.FirstOrDefault(m => m.Id == message.Id);
		if (member != null)
		{
			_currentParty.Remove(member);
		}
	}

	private void OnInitializeExpeditionPartyForClient(Message_InitializeExpeditionPartyForClient message, ulong senderClientId)
	{
		_currentId = message.CurrentId;
		_currentParty.Clear();
		foreach (var member in message.CurrentParty)
		{
			_currentParty.Add(member);
		}
	}

	private void ClientTracker_OnClientConnectedToUs(ulong clientId)
	{
		_netEventBus.SendToOne(new Message_InitializeExpeditionPartyForClient { CurrentId = _currentId, CurrentParty = _currentParty.ToList() }, clientId);
	}
	private void ClientTracker_OnClientDisconnectedFromUs(ulong clientId)
	{
		var membersToRemove = _currentParty.Where(m => m.ClientId == clientId).ToList();
		foreach (var member in membersToRemove)
		{
			_netEventBus.SendToAll(new Message_RemoveExpeditionPartyMember { Id = member.Id });
		}
	}
}

public struct Message_AddExpeditionPartyMember : INetMessage
{
	public SerializableCustomizationData CustomizationData;
	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableFragmentedSequenced;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeCustomizationData(ref CustomizationData);
	}
}

public struct Message_RemoveExpeditionPartyMember : INetMessage
{
	public uint Id;
	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableFragmentedSequenced;
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref Id);
	}
}

public struct Message_InitializeExpeditionPartyForClient : INetMessage
{
	public uint CurrentId;
	public List<ExpeditionPartyMember> CurrentParty;
	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableFragmentedSequenced;
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref CurrentId);
		if (serializer.IsWriter)
		{
			int partyLength = CurrentParty.Count;
			serializer.SerializeValue(ref partyLength);
			for (int i = 0; i < partyLength; i++)
			{
				CurrentParty[i].NetworkSerialize(serializer);
			}
		}
		else
		{
			int partyLength = 0;
			serializer.SerializeValue(ref partyLength);
			CurrentParty = new List<ExpeditionPartyMember>(partyLength);
			for (int i = 0; i < partyLength; i++)
			{
				var member = new ExpeditionPartyMember(0, 0, null);
				member.NetworkSerialize(serializer);
				CurrentParty.Add(member);
			}
		}
	}
}