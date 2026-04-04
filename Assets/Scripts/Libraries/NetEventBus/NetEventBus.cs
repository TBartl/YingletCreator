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

	// Provided for convenience even though it's out of this class's scope
	public double NetworkTime { get; }
}

public class NetEventBus : MonoBehaviour, INetEventBus
{
	private NetworkManager _networkManager;
	private INetClientTracker _clientTracker;
	private NetMessageRegistry _messageRegistry;
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
		var messagingManager = _networkManager.CustomMessagingManager;
		if (_networkManager.IsServer || messagingManager == null)
		{
			// Send to ourselves if we're the server or if we're not running
			FireMessageToListeners(message, sender);
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

		using var writer = new FastBufferWriter(128, Unity.Collections.Allocator.Temp);

		// 1. Write the message type ID
		var messageId = _messageRegistry.GetMessageId(message);
		writer.WriteValueSafe(messageId);

		// 2. Write the sender
		writer.WriteValueSafe((uint)sender);

		// 3. Write the message data
		writer.WriteNetworkSerializable(message);

		if (_networkManager.IsServer)
		{
			// Send it to everyone excluding ourselves
			var clients = _networkManager.ConnectedClientsIds.Where(c => c != NetworkManager.ServerClientId).ToList();

			messagingManager.SendUnnamedMessage(clients, writer);
		}
		else
		{
			// Send it to the server so it can relay it
			messagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
		}
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
