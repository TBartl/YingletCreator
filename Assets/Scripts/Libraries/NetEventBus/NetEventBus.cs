using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public delegate void NetMessageCallback<T>(T message, ulong senderClientId) where T : INetMessage;

/// <summary>
/// This tries to mimic the FishNet broadcast system.
/// I liked it in theory, but couldn't pursue FishNet due to the lack of a offline transport w/o paying
/// </summary>
public interface INetEventBus
{
	public void Subscribe<T>(NetMessageCallback<T> callback) where T : INetMessage;
	public void Unsubscribe<T>(NetMessageCallback<T> callback) where T : INetMessage;

	public void SendToAll<T>(T message) where T : INetMessage;
	public void SendToOne<T>(T message, ulong targetConnectionId) where T : INetMessage;

	// Provided for convenience even though it's out of this class's scope
	public double NetworkTime { get; }
}

public class NetEventBus : MonoBehaviour, INetEventBus
{
	private NetworkManager _networkManager;
	private INetClientTracker _clientTracker;
	private INetMessageRegistry _messageRegistry;
	private readonly Dictionary<Type, Delegate> _subscribers = new();

	private void Awake()
	{
		_clientTracker = this.GetComponent<INetClientTracker>();
		_messageRegistry = new NetMessageRegistry();
	}

	void Start()
	{
		_networkManager = NetworkManager.Singleton;
		_networkManager.OnServerStarted += OnServerStarted;
		_networkManager.OnClientConnectedCallback += OnClientConnected;
	}

	private void OnDestroy()
	{
		_networkManager.OnServerStarted -= OnServerStarted;
		_networkManager.OnClientConnectedCallback -= OnClientConnected;

		if (_networkManager.CustomMessagingManager != null)
		{
			_networkManager.CustomMessagingManager.OnUnnamedMessage -= CustomMessagingManager_OnUnnamedMessage;
		}
	}
	private void OnServerStarted()
	{
		RegisterUnnamedMessage();
	}

	private void OnClientConnected(ulong obj)
	{
		if (!_networkManager.IsPureClient()) return;
		RegisterUnnamedMessage();
	}

	private void RegisterUnnamedMessage()
	{
		_networkManager.CustomMessagingManager.OnUnnamedMessage += CustomMessagingManager_OnUnnamedMessage;
	}

	public double NetworkTime => _networkManager.NetworkTimeSystem?.ServerTime ?? Time.timeAsDouble;

	public void SendToAll<T>(T message) where T : INetMessage
	{
		SendToAll(message, _clientTracker.LocalClientID);
	}

	private void SendToAll<T>(T message, ulong sender) where T : INetMessage
	{
		// For servers, send to everyone except ourselves and the sender
		// For clients, send to the server only so we can relay it
		var clients = GetRecipients();
		Send(message, sender, clients);

		List<ulong> GetRecipients()
		{
			if (_networkManager.IsServer)
			{
				if (message.SendToSelf)
				{
					return _networkManager.ConnectedClientsIds.ToList();
				}
				// Client that sent it shouldn't end up recieving it
				return _networkManager.ConnectedClientsIds.Where(c => c != sender).ToList();
			}
			else
			{
				return new List<ulong> { NetworkManager.ServerClientId };
			}
		}
	}

	private void Send<T>(T message, ulong sender, IReadOnlyList<ulong> recipients) where T : INetMessage
	{
		// Keeping this around a bit longer in case I need to debug it more
		//if (message.DeliveryMethod != NetworkDelivery.Unreliable)
		//{
		//	Debug.Log($"Sending message of type {typeof(T)} from client {sender} to recipients: {string.Join(", ", recipients)}");
		//}

		if (recipients.Count == 0)
		{
			return;
		}

		var messagingManager = _networkManager.CustomMessagingManager;
		if ((_networkManager.IsServer || messagingManager == null) && recipients.Contains(NetworkManager.ServerClientId))
		{
			// Send to ourselves if we're the server or if we're not running
			FireMessageToListeners(message, sender);

			if (recipients.Count == 1)
			{
				// We were the only recipient, no need to send a network message
				return;
			}

			// Update the recipients list to not send to ourselves again when we actually send the networked message
			recipients = recipients.Where(r => r != NetworkManager.ServerClientId).ToList();
		}

		if (messagingManager == null)
		{
			// No network connection, no point sending
			return;
		}

		if (_networkManager.IsServer && _networkManager.ConnectedClientsIds.Count == 1)
		{
			// We're a server but no one is connected, no point sending
			return;
		}

		using var writer = new FastBufferWriter(128, Unity.Collections.Allocator.Temp, int.MaxValue);

		// 1. Write the message type ID
		var messageId = _messageRegistry.GetMessageId(message);
		writer.WriteValueSafe(messageId);

		// 2. Write the sender
		writer.WriteValueSafe((uint)sender);

		// 3. Write the message data
		writer.WriteNetworkSerializable(message);


		if (recipients.Count == 1)
		{
			// We need to do this because clients seemingly can't use the List version of SendUnnamedMessage, even with a list of 1
			messagingManager.SendUnnamedMessage(recipients[0], writer, message.DeliveryMethod);
		}
		else
		{
			messagingManager.SendUnnamedMessage(recipients, writer, message.DeliveryMethod);

		}
	}

	public void SendToOne<T>(T message, ulong targetConnectionId) where T : INetMessage
	{
		if (!_networkManager.IsServer)
		{
			Debug.LogWarning("SendToOne is only supported on the server");
			return;
		}
		Send(message, _clientTracker.LocalClientID, new List<ulong> { targetConnectionId });
	}

	// Custom message format
	private void CustomMessagingManager_OnUnnamedMessage(ulong actualSenderClientId, FastBufferReader reader)
	{
		// 1. Read the type 
		reader.ReadValueSafe(out uint messageId);

		// 2. Read the sender
		reader.ReadValueSafe(out uint attachedSenderId);

		// 3. Read the message data
		var message = _messageRegistry.ReadMessage(messageId, ref reader);
		//Debug.Log($"Received message of type {message.GetType()} from client {senderClientId}");

		if (actualSenderClientId == 0)
		{
			if (_networkManager.IsPureClient())
			{
				// Got a message from the server, fire events
				FireMessageToListeners(message, attachedSenderId);
			}
			else
			{
				Debug.LogWarning("Got a message from the server as the server - this shouldn't happen");
			}
		}
		else
		{
			if (_networkManager.IsServer)
			{
				// We got this from a client, relay it to everyone
				SendToAll(message, attachedSenderId);
			}
			else
			{
				Debug.LogWarning("Got a message from a client as a client - this shouldn't happen");
			}
		}
	}

	private void FireMessageToListeners<T>(T message, ulong senderId) where T : INetMessage
	{
		var type = message.GetType();
		if (_subscribers.TryGetValue(type, out var del))
		{
			foreach (Delegate d in del.GetInvocationList())
			{
				d.DynamicInvoke(message, senderId);
			}
		}
	}


	public void Subscribe<T>(NetMessageCallback<T> callback) where T : INetMessage
	{
		var type = typeof(T);

		if (_subscribers.TryGetValue(type, out var existing))
		{
			_subscribers[type] = Delegate.Combine(existing, callback);
		}
		else
		{
			_subscribers[type] = callback;
		}
	}

	public void Unsubscribe<T>(NetMessageCallback<T> callback) where T : INetMessage
	{
		var type = typeof(T);
		if (_subscribers.TryGetValue(type, out var existing))
		{
			var combined = Delegate.Remove(existing, callback);
			if (combined == null)
			{
				_subscribers.Remove(type);
			}
			else
			{
				_subscribers[type] = combined;
			}
		}
	}
}
