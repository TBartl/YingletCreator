using Character.Creator;
using Unity.Netcode;
using UnityEngine;

public class NetworkCustomizationData : MonoBehaviour
{
	private INetStateReader _netStateTracker;
	private ICharacterCreatorTracker _characterCreatorTracker;
	private INetEventBus _eventBus;
	private IGameCharacterDataRepository _dataRepo;
	private IPlayerIdentity _identity;

	void Start()
	{
		_netStateTracker = Singletons.GetSingleton<INetStateReader>();
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_dataRepo = this.GetComponent<IGameCharacterDataRepository>();
		_identity = this.GetComponentInParent<IPlayerIdentity>();

		_characterCreatorTracker.IsInCharacterCreator.OnChanged += InCharacterCreator_OnChanged;
		_eventBus.Subscribe<Message_UpdateCustomizationData>(OnCustomizationDataUpdated);
	}

	private void OnDestroy()
	{
		_characterCreatorTracker.IsInCharacterCreator.OnChanged -= InCharacterCreator_OnChanged;
	}

	private void InCharacterCreator_OnChanged(bool from, bool to)
	{
		if (!_netStateTracker.IsInAnyState) return; // Optimization: Don't do anything if we're not in a net state

		if (!_identity.IsMine) return;
		if (to) return; // Only act when we're leaving it

		var data = _dataRepo.CustomizationData;
		var serialized = new SerializableCustomizationData(data);

		var message = new Message_UpdateCustomizationData(JsonUtility.ToJson(serialized));
		_eventBus.SendToAll(message);

	}

	private void OnCustomizationDataUpdated(Message_UpdateCustomizationData message, ulong senderClientId)
	{
		if (_identity.IsMine) return; // We already know, return
		if (_identity.ConnectionId != senderClientId) return; // Not for us, return

		var deserialized = JsonUtility.FromJson<SerializableCustomizationData>(message.JSONData);
		_dataRepo.ForceCustomizationData(deserialized);
	}
}

public struct Message_UpdateCustomizationData : INetMessage
{
	public string JSONData;

	public Message_UpdateCustomizationData(string jsonData)
	{
		JSONData = jsonData;
	}

	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableFragmentedSequenced;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref JSONData);
	}
}
