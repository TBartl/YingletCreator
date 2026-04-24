using Character.Creator;
using Networking;
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
	private ICharacterIdentity _identity;
	private bool _wantsToSendInitDataToServer = false;

	void Awake()
	{
		_netStateTracker = Singletons.GetSingleton<INetStateReader>();
		_netClientTracker = Singletons.GetSingleton<INetClientTracker>();
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_dataRepo = this.GetComponent<IGameCharacterDataRepository>();
		_identity = this.GetComponentInParentSafe<ICharacterIdentity>();

		_netClientTracker.OnClientConnectedToUs += NetClientTracker_OnClientConnectedToUs;
		_netClientTracker.OnConnectedToServer += NetClientTracker_OnConnectedToServer;
		_characterCreatorTracker.IsInCharacterCreator.OnChanged += InCharacterCreator_OnChanged;
		_eventBus.Subscribe<Message_UpdateCustomizationData>(OnCustomizationDataUpdated);
		_identity.NetIdObservable.OnChanged += OwnerNetIdObservable_OnChanged;
	}

	private void OnDestroy()
	{
		_netClientTracker.OnClientConnectedToUs -= NetClientTracker_OnClientConnectedToUs;
		_netClientTracker.OnConnectedToServer -= NetClientTracker_OnConnectedToServer;
		_characterCreatorTracker.IsInCharacterCreator.OnChanged -= InCharacterCreator_OnChanged;
		_eventBus.Unsubscribe<Message_UpdateCustomizationData>(OnCustomizationDataUpdated);
		_identity.NetIdObservable.OnChanged -= OwnerNetIdObservable_OnChanged;

	}

	private void NetClientTracker_OnClientConnectedToUs(ulong clientId)
	{
		var message = CreateMessage();
		_eventBus.SendToOne(message, clientId);
	}

	private void NetClientTracker_OnConnectedToServer(ulong connectionId)
	{
		// We can't do this yet because we don't know our new IDs
		_wantsToSendInitDataToServer = true;
	}

	private void OwnerNetIdObservable_OnChanged(ulong from, ulong to)
	{
		if (!_wantsToSendInitDataToServer) return;
		_eventBus.SendToAll(CreateMessage());
		_wantsToSendInitDataToServer = true;
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
		if (senderClientId != _identity.OwnerClientId) return; // Not from the owner, return
		if (message.NetId != _identity.NetId) return; // Not for this character, return

		var deserialized = JsonUtility.FromJson<SerializableCustomizationData>(message.JSONData);
		_dataRepo.ForceCustomizationData(deserialized);
	}

	public Message_UpdateCustomizationData CreateMessage()
	{
		var data = _dataRepo.CustomizationData;
		var serialized = new SerializableCustomizationData(data);
		return new Message_UpdateCustomizationData(_identity.NetId, JsonUtility.ToJson(serialized));
	}
}

public struct Message_UpdateCustomizationData : INetMessage
{
	public ulong NetId;
	public string JSONData;

	public Message_UpdateCustomizationData(ulong netId, string jsonData)
	{
		NetId = netId;
		JSONData = jsonData;
	}

	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableFragmentedSequenced;
	public bool SendToSelf => false;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
		serializer.SerializeValue(ref JSONData);
	}
}