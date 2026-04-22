using Reactivity;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public delegate void ClientConnectionEvent(ulong clientId);


public struct ClientData
{
	public ClientData(ulong localClientId, IEnumerable<ulong> clientIds)
	{
		LocalClientID = localClientId;
		ClientIDs = clientIds;
	}

	public ulong LocalClientID { get; }
	public IEnumerable<ulong> ClientIDs { get; }
}

public interface INetClientTracker
{
	/// <summary>
	/// Our own client's ID
	/// Will be 0 until we connect to a host
	/// </summary>
	public ulong LocalClientID { get; }

	/// <summary>
	/// All of the data associated with clients
	/// Kept as a single property for better observability compatibility (the client ID and the clientIDs can change in unison)
	/// </summary>
	ClientData Data { get; }

	/// <summary>
	/// Called only for servers when a client connects;
	/// </summary>
	event ClientConnectionEvent OnClientConnectedToUs;

	/// <summary>
	/// Called only for servers when a client disconnects
	/// </summary>
	event ClientConnectionEvent OnClientDisconnectedFromUs;

	/// <summary>
	/// Called only when a pure client connects to a server
	/// </summary>
	event ClientConnectionEvent OnConnectedToServer;
}

internal sealed class NetClientTracker : ReactiveBehaviour, INetClientTracker
{
	private INetStateReader _netState;
	private INetEventBus _eventBus;
	private NetworkManager _netManager;

	Observable<ClientData> _data = new Observable<ClientData>(new ClientData(0, new ulong[] { 0 }));
	Computed<ulong> _computedLocalClientId; // Compute for less refires in the average case when a client is just connecting / disconnecting

	public event ClientConnectionEvent OnClientConnectedToUs = delegate { };
	public event ClientConnectionEvent OnClientDisconnectedFromUs = delegate { };
	public event ClientConnectionEvent OnConnectedToServer = delegate { };

	public ulong LocalClientID => _computedLocalClientId.Val;

	public ClientData Data => _data.Val;


	private void Awake()
	{
		_netState = this.GetComponent<INetStateReader>();
		_eventBus = this.GetComponent<INetEventBus>();
		_computedLocalClientId = CreateComputed(() => _data.Val.LocalClientID);

		_netState.OnVoluntaryClientDisconnection += OnVoluntaryClientDisconnection;
	}

	private void Start()
	{
		_netManager = NetworkManager.Singleton;

		_netManager.OnClientConnectedCallback += OnClientConnected;
		_netManager.OnClientDisconnectCallback += OnClientDisconnected;
		_eventBus.Subscribe<Message_UpdateClientManifest>(OnClientManifestUpdated);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		_netManager.OnClientConnectedCallback -= OnClientConnected;
		_netManager.OnClientDisconnectCallback -= OnClientDisconnected;
		_eventBus.Unsubscribe<Message_UpdateClientManifest>(OnClientManifestUpdated);
		_netState.OnVoluntaryClientDisconnection -= OnVoluntaryClientDisconnection;
	}

	private void OnClientManifestUpdated(Message_UpdateClientManifest manifest, ulong senderClientId)
	{
		_data.Val = new ClientData(_data.Val.LocalClientID, manifest.ClientIds);
	}

	private void OnClientConnected(ulong clientId)
	{
		Debug.Log($"Client connected: {clientId}");

		if (_netManager.IsPureClient())
		{
			// Called when we connect for the first time
			// Use it to update our client ID
			_data.Val = new ClientData(clientId, new[] { clientId });
			OnConnectedToServer.Invoke(clientId);
			return;
		}
		else if (_netManager.IsServer)
		{
			if (clientId == 0) return; // Ignore ourselves connecting as a host
			ServerSendUpdatedManifest();
			OnClientConnectedToUs.Invoke(clientId);
		}
		else
		{
			Debug.LogWarning("Received client connected callback, but we're neither a client nor a server. This shouldn't happen");
		}
	}

	private void OnClientDisconnected(ulong clientId)
	{
		Debug.Log($"Client disconnected: {clientId}");
		if (_netManager.IsServer && clientId != 0)
		{
			ServerSendUpdatedManifest();
			OnClientDisconnectedFromUs.Invoke(clientId);
		}
		else
		{
			// We disconnected; reset to default state
			_data.Val = new ClientData(0, new ulong[] { 0 });
		}
	}

	private void OnVoluntaryClientDisconnection()
	{
		// We disconnected; reset to default state
		_data.Val = new ClientData(0, new ulong[] { 0 });
	}

	void ServerSendUpdatedManifest()
	{
		var message = new Message_UpdateClientManifest()
		{
			ClientIds = _netManager.ConnectedClientsIds.ToArray()
		};
		_eventBus.SendToAll(message);
	}
}
