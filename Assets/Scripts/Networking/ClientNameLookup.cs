using Reactivity;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

public interface IClientNameLookup
{
	string GetNameForClient(ulong clientId);
}

public class ClientNameLookup : MonoBehaviour, IClientNameLookup
{
	const string Unknown = "Unknown";
	private IToastManager _toastManager;
	private INetStateReader _netState;
	private INetEventBus _netEventBus;

	// I originally tried to get this from the lobby, but only the host really has access to the information it'd need for that
	// Doing it by hand also leaves me open to non-steam solutions too
	ObservableDict<ulong, string> _clientIdToNameCache = new ObservableDict<ulong, string>();

	void Awake()
	{
		_toastManager = Singletons.GetSingleton<IToastManager>();
		_netState = Singletons.GetSingleton<INetStateReader>();
		_netEventBus = Singletons.GetSingleton<INetEventBus>();

		_netState.OnConnectedToServer += NetState_OnConnectedToServer;
		_netState.OnClientConnectedToUs += NetState_OnClientConnectedToUs;
		_netState.OnLocalDisconnected += NetState_OnLocalDisconnected;

		_netEventBus.Subscribe<Message_SendClientName>(NetEventBus_OnSendClientName);

		ResetNames();
	}

	private void OnDestroy()
	{
		_netState.OnConnectedToServer -= NetState_OnConnectedToServer;
		_netState.OnClientConnectedToUs -= NetState_OnClientConnectedToUs;
		_netState.OnLocalDisconnected -= NetState_OnLocalDisconnected;

		_netEventBus.Unsubscribe<Message_SendClientName>(NetEventBus_OnSendClientName);
	}

	float _lastShowTime = 0;
	private void NetEventBus_OnSendClientName(Message_SendClientName message, ulong senderClientId)
	{
		_clientIdToNameCache[message.ClientId] = message.Name;

		// This is out of responsibility for this class but w/e
		if (senderClientId != _netState.LocalClientID && Time.time > _lastShowTime + .05f)
		{
			_toastManager.Show($"'{message.Name}' connected.");
			_lastShowTime = Time.time;
		}
	}

	private void NetState_OnConnectedToServer(ulong id)
	{
		_netEventBus.SendToAll(new Message_SendClientName()
		{
			ClientId = id,
			Name = GetMySteamName()
		});
	}

	private void NetState_OnClientConnectedToUs(ulong clientId)
	{
		// Send the client all the names we have
		foreach (var kvp in _clientIdToNameCache)
		{
			_netEventBus.SendToOne(new Message_SendClientName()
			{
				ClientId = kvp.Key,
				Name = kvp.Value
			}, clientId);
		}
	}

	private void NetState_OnLocalDisconnected()
	{
		ResetNames();
	}

	void ResetNames()
	{
		_clientIdToNameCache.Clear();
		_clientIdToNameCache.Add(0, GetMySteamName());
	}

	string GetMySteamName()
	{
		try
		{
			return SteamClient.Name;
		}
		catch
		{
			Debug.LogWarning("Failed to get Steam name for local client. Defaulting to Unknown.");
			return Unknown;
		}
	}

	public string GetNameForClient(ulong clientId)
	{
		if (_clientIdToNameCache.TryGetValue(clientId, out var name))
		{
			return name;
		}
		else
		{
			return Unknown;
		}
	}
}

struct Message_SendClientName : INetMessage
{
	public ulong ClientId;
	public string Name;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref ClientId);
		serializer.SerializeValue(ref Name);
	}
}