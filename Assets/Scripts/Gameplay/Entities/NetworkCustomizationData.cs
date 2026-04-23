using Character.Creator;
using Unity.Netcode;
using UnityEngine;

public interface INetworkCustomizationData
{
	Message_UpdateCustomizationData CreateMessage();
}

public class NetworkCustomizationData : MonoBehaviour, INetworkCustomizationData
{
	private INetStateReader _netStateTracker;
	private INetClientTracker _netClientTracker;
	private ICharacterCreatorTracker _characterCreatorTracker;
	private INetEventBus _eventBus;
	private IGameCharacterDataRepository _dataRepo;
	private IPlayerIdentity _identity;

	void Awake()
	{
		_netStateTracker = Singletons.GetSingleton<INetStateReader>();
		_netClientTracker = Singletons.GetSingleton<INetClientTracker>();
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_dataRepo = this.GetComponent<IGameCharacterDataRepository>();
		_identity = this.GetComponentInParent<IPlayerIdentity>();

		_netClientTracker.OnConnectedToServer += NetClientTracker_OnConnectedToServer;
		_characterCreatorTracker.IsInCharacterCreator.OnChanged += InCharacterCreator_OnChanged;
		_eventBus.Subscribe<Message_UpdateCustomizationData>(OnCustomizationDataUpdated);
	}

	private void OnDestroy()
	{
		_netClientTracker.OnConnectedToServer -= NetClientTracker_OnConnectedToServer;
		_characterCreatorTracker.IsInCharacterCreator.OnChanged -= InCharacterCreator_OnChanged;
		_eventBus.Unsubscribe<Message_UpdateCustomizationData>(OnCustomizationDataUpdated);
	}

	private void NetClientTracker_OnConnectedToServer(ulong connectionId)
	{
		if (!_identity.IsMine) return;

		var message = CreateMessage();
		_eventBus.SendToAll(message);
	}

	private void InCharacterCreator_OnChanged(bool from, bool to)
	{
		if (!_netStateTracker.IsInAnyState) return; // Optimization: Don't do anything if we're not in a net state

		if (!_identity.IsActive) return;
		if (to) return; // Only act when we're leaving it

		var message = CreateMessage();
		_eventBus.SendToAll(message);

	}

	private void OnCustomizationDataUpdated(Message_UpdateCustomizationData message, ulong senderClientId)
	{
		if (_identity.IsMine) return; // We already know, return
		if (_identity.ConnectionId != message.ClientId) return; // Not for us, return

		var deserialized = JsonUtility.FromJson<SerializableCustomizationData>(message.JSONData);
		_dataRepo.ForceCustomizationData(deserialized);
	}

	public Message_UpdateCustomizationData CreateMessage()
	{
		var data = _dataRepo.CustomizationData;
		var serialized = new SerializableCustomizationData(data);
		return new Message_UpdateCustomizationData(_identity.ConnectionId, JsonUtility.ToJson(serialized));
	}
}

public struct Message_UpdateCustomizationData : INetMessage
{
	public ulong ClientId;
	public string JSONData;

	public Message_UpdateCustomizationData(ulong clientId, string jsonData)
	{
		ClientId = clientId;
		JSONData = jsonData;
	}

	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableFragmentedSequenced;
	public bool SendToSelf => false;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref ClientId);
		serializer.SerializeValue(ref JSONData);
	}
}