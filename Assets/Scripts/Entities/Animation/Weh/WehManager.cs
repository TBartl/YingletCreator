using Networking;
using System;
using Unity.Netcode;
using UnityEngine;

public interface IWehManager
{
	event Action OnWeh;
}

public class WehManager : MonoBehaviour, IWehManager
{
	public event Action OnWeh;

	private ICharacterIdentity _playerIdentity;
	private INetEventBus _eventBus;

	private void Start()
	{
		_playerIdentity = this.GetCharacterRootComponent<ICharacterIdentity>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();

		_eventBus.Subscribe<Message_Weh>(OnMessagePlayWeh);
	}

	private void OnDestroy()
	{
		_eventBus.Unsubscribe<Message_Weh>(OnMessagePlayWeh);
	}

	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.V)) return;
		if (!_playerIdentity.IsActiveAndMine) return;

		// Play immediately on the client
		OnWeh?.Invoke();

		// Send message to other clients
		_eventBus.SendToAll(new Message_Weh(_playerIdentity.NetId));
	}

	private void OnMessagePlayWeh(Message_Weh message, ulong senderClientId)
	{
		if (_playerIdentity.IsMine) return; // We already played it, return
		if (senderClientId != _playerIdentity.OwnerClientId) return; // Not from the owner, return
		if (message.NetId != _playerIdentity.NetId) return; // Not for this character, return

		OnWeh?.Invoke();
	}
}

public struct Message_Weh : INetMessage
{
	public ulong NetId;

	public Message_Weh(ulong netId)
	{
		NetId = netId;
	}

	public NetworkDelivery DeliveryMethod => NetworkDelivery.Reliable;
	public bool SendToSelf => false;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
	}
}
