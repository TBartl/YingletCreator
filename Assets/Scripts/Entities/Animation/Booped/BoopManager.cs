using Networking;
using Reactivity;
using System;
using Unity.Netcode;
using UnityEngine;

public interface IBoopManager
{
	event Action OnBoop;
}

public class BoopManager : ReactiveBehaviour, IBoopManager
{
	public event Action OnBoop;

	private IColliderHoverManager _colliderHoverManager;
	private Computed<bool> _hoveringBoopHitbox;
	private ICharacterIdentity _playerIdentity;
	private INetEventBus _eventBus;

	private void Start()
	{
		_colliderHoverManager = Singletons.GetSingleton<IColliderHoverManager>();
		_playerIdentity = this.GetCharacterRootComponent<ICharacterIdentity>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();

		_eventBus.Subscribe<Message_Boop>(OnMessageBoop);

		_hoveringBoopHitbox = CreateComputed(ComputeHoveringBoopHitbox);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		_eventBus.Unsubscribe<Message_Boop>(OnMessageBoop);
	}

	private bool ComputeHoveringBoopHitbox()
	{
		var hovered = _colliderHoverManager.CurrentlyHovered;
		if (hovered == null) return false;
		return hovered.gameObject.GetComponent<BoopHitbox>() != null;
	}

	void Update()
	{
		if (!Input.GetMouseButtonDown(0)) return;

		// Just let anyone boop anyone lul
		//if (!_playerIdentity.IsMine) return;

		if (!_hoveringBoopHitbox.Val) return;

		// Play immediately on the client
		OnBoop?.Invoke();

		// Send message to other clients
		_eventBus.SendToAll(new Message_Boop(_playerIdentity.NetId));
	}

	private void OnMessageBoop(Message_Boop message, ulong senderClientId)
	{
		if (_playerIdentity.IsMine) return; // We already played it, return

		// Just let anyone boop anyone lul
		//if (senderClientId != _playerIdentity.OwnerClientId) return; // Not from the owner, return

		if (message.NetId != _playerIdentity.NetId) return; // Not for this character, return

		OnBoop?.Invoke();
	}
}

public struct Message_Boop : INetMessage
{
	public ulong NetId;

	public Message_Boop(ulong netId)
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
