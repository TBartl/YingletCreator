using Reactivity;
using System;
using Unity.Netcode;

/// <summary>
/// NetworkManager is not under our control. This serves as a thin layer to expose the state of it but with reactivity in mind
/// </summary>
public interface INetStateProvider
{
	bool IsHost { get; }
	bool IsAttemptingClient { get; }
	bool IsConnectedClient { get; }
	bool IsRunning { get; }

	/// <summary>
	/// Called when either a host stops hosting, or a client DCs
	/// </summary>
	event Action OnDisconnect;
}

public class NetStateProvider : ReactiveBehaviour, INetStateProvider
{
	Observable<bool> _isHost = new(false);
	Observable<bool> _isAttemptingClient = new(false);
	Observable<bool> _isConnectingClient = new(false);
	Computed<bool> _isRunning;
	private NetworkManager _netManager;

	public event Action OnDisconnect = delegate { };

	public bool IsHost => _isHost.Val;
	public bool IsAttemptingClient => _isAttemptingClient.Val;
	public bool IsConnectedClient => _isConnectingClient.Val;
	public bool IsRunning => _isRunning.Val;


	private void Awake()
	{
		_isRunning = CreateComputed(ComputeIsRunning);
	}

	private void Start()
	{
		_netManager = NetworkManager.Singleton;

		// Listen for events to set IsHost and IsClient appropriately
		_netManager.OnServerStarted += OnServerStarted;
		_netManager.OnClientConnectedCallback += OnClientConnected;
		_netManager.OnClientDisconnectCallback += OnClientDisconnected;
	}

	private void Update()
	{
		// No callback for this, so we just have to poll for it :/
		_isAttemptingClient.Val = _netManager.IsClient && !_netManager.IsConnectedClient;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_netManager == null) return;
		_netManager.OnServerStarted -= OnServerStarted;
		_netManager.OnClientConnectedCallback -= OnClientConnected;
		_netManager.OnClientDisconnectCallback -= OnClientDisconnected;
	}

	private void OnServerStarted()
	{
		_isHost.Val = true;
	}

	private void OnClientConnected(ulong clientId)
	{
		if (clientId == NetworkManager.ServerClientId)
		{
			// We just started hosting
			_isHost.Val = true;
		}
		else if (clientId == _netManager.LocalClientId)
		{
			// We ourselves just connected as a client
			_isConnectingClient.Val = true;
			_isAttemptingClient.Val = false;
		}
	}

	private void OnClientDisconnected(ulong clientId)
	{
		if (_isHost.Val)
		{
			if (clientId == NetworkManager.ServerClientId)
			{
				// We see ourselves disconnected
				_isHost.Val = false;
				OnDisconnect();
			}
		}
		else
		{
			// Otherwise, this must have been us as the client disconnecting
			_isConnectingClient.Val = false;
			_isAttemptingClient.Val = false;
			OnDisconnect();
		}
	}

	private bool ComputeIsRunning()
	{
		return _isHost.Val || _isConnectingClient.Val;
	}
}
