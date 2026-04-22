using Assets.Scripts.Entities.Character.Creator.Data;
using Character.Creator;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public sealed class ExpeditionPartyMember : INetworkSerializable
{
	public ExpeditionPartyMember(uint id, SerializableCustomizationData customizationData, ulong clientId)
	{
		Id = id;
		CustomizationData = customizationData;
		ClientId = clientId;
	}

	public SerializableCustomizationData CustomizationData;
	public uint Id;
	public ulong ClientId;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref Id);
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

	public IList<ExpeditionPartyMember> CurrentParty => _currentParty;

	private void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_netEventBus = Singletons.GetSingleton<INetEventBus>();

		_netState.OnLocalDisconnected += NetState_OnLocalDisconnected;
		_netEventBus.Subscribe<Message_AddExpeditionPartyMember>(OnAddExpeditionPartyMember);
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
		var member = _currentParty.FirstOrDefault(m => m.Id == id);
		if (member != null)
		{
			_currentParty.Remove(member);
		}
	}

	private void OnAddExpeditionPartyMember(Message_AddExpeditionPartyMember message, ulong senderClientId)
	{

		if (_currentParty.Count < MAX_CHARACTERS)
		{
			var newMember = new ExpeditionPartyMember(_currentId++, message.CustomizationData, senderClientId);
			_currentParty.Add(newMember);
		}
		else
		{
			Debug.LogWarning("Max party size reached");
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
	//public List<ExpeditionPartyMember> CurrentParty;
	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableFragmentedSequenced;
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		//serializer.SerializeList(ref CurrentParty);
	}
}