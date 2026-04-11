using Netcode.Transports.Facepunch;
using Reactivity;
using Steamworks;
using Steamworks.Data;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// NetworkManager is not under our control. This serves as a thin layer to expose the state of it but with reactivity in mind
/// </summary>
public interface INetStateReader
{
	/// <summary>
	/// While NetworkManager starts hosting synchronously, we need a lobby before we're considered fully hosting
	/// This is when we are in that state of partial hosting
	/// </summary>
	bool IsAttemptingHost { get; }

	/// <summary>
	/// This is when the lobby is open and we are running as a host
	/// </summary>
	bool IsConnectedHost { get; }

	/// <summary>
	/// This means one of two things:
	/// 1) We are still attempting to join the lobby
	/// 2) We are still attempting to connect
	/// </summary>
	bool IsAttemptingClient { get; }

	/// <summary>
	/// This is when we are fully connected as a client through the lobby
	/// </summary>
	bool IsConnectedClient { get; }

	/// <summary>
	/// Returns true if the user is attempting to host, hosting, attempting to connect as a client, or connected as a client
	/// </summary>
	bool IsInAnyState { get; }

	/// <summary>
	/// The currently connected lobby for either hosts or clients
	/// </summary>
	Lobby? CurrentLobby { get; }

	event Action OnVoluntaryClientDisconnection;
}

public interface INetStateWriter : INetStateReader
{
	/// <summary>
	/// Attempts to start hosting
	/// While the NetworkManager starts hosting synchronously, we need a lobby before we're considered fully hosting
	/// </summary>
	void StartHost();

	/// <summary>
	/// Attempts to connect to a lobby via ID
	/// </summary>
	void StartConnectToLobby(ulong lobbyId);

	/// <summary>
	/// Disconnects everything and leaves any lobby
	/// </summary>
	void Disconnect();
}

public class NetStateManager : ReactiveBehaviour, INetStateWriter
{
	public const int MAX_PLAYERS = 64;

	[SerializeField] bool _steam;
	[SerializeField] CustomFacepunchTransport _steamTransport;
	[SerializeField] NetworkTransport _debugTransport;

	private NetworkManager _netManager;

	// Host
	Observable<bool> _isNetManagerHosted = new(false);
	private IToastManager _toastManager;
	Computed<bool> _isAttemptingHost;
	Computed<bool> _isConnectedHost;

	// Client
	Observable<bool> _isNetManagerClientAttempting = new(false);
	Observable<bool> _isNetManagerClientConnected = new(false);
	Observable<bool> _isClientAttemptingToJoinLobby = new(false);
	Computed<bool> _isAttemptingClient;
	Computed<bool> _isConnectedClient;

	Computed<bool> _isInAnyState;

	Observable<Lobby?> _currentLobby = new();

	public event Action OnVoluntaryClientDisconnection = delegate { };

	// If a user Disconnects while attempting to start a lobby, we should get rid of the lobby we created
	private uint _validLobbyIndex = 0;

	private void Awake()
	{
		_toastManager = Singletons.GetSingleton<IToastManager>();

		_isAttemptingHost = CreateComputed(() => _isNetManagerHosted.Val && (_currentLobby.Val == null || !_steam));
		_isConnectedHost = CreateComputed(() => _isNetManagerHosted.Val && (_currentLobby.Val != null || !_steam));
		_isAttemptingClient = CreateComputed(() => _isNetManagerClientAttempting.Val || _isClientAttemptingToJoinLobby.Val); // Either netmanager isn't ready or lobby isn't
		_isConnectedClient = CreateComputed(() => _isNetManagerClientConnected.Val && (_currentLobby.Val != null || !_steam));
		_isInAnyState = CreateComputed(() => IsAttemptingHost || IsConnectedHost || IsAttemptingClient || IsConnectedClient);
	}

	private void Start()
	{
		_netManager = NetworkManager.Singleton;

		_netManager.NetworkConfig.NetworkTransport = _steam ? _steamTransport : _debugTransport;

		// Listen for events to set IsHost and IsClient appropriately
		_netManager.OnClientConnectedCallback += OnClientConnected;
		_netManager.OnClientDisconnectCallback += OnClientDisconnected;
		SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
	}

	private void Update()
	{
		// No callback for this, so we just have to poll for it :/
		_isNetManagerClientAttempting.Val = _netManager.IsClient && !_netManager.IsConnectedClient;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_netManager != null) return;
		{
			_netManager.OnClientConnectedCallback -= OnClientConnected;
			_netManager.OnClientDisconnectCallback -= OnClientDisconnected;
		}
		SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
	}


	public bool IsAttemptingHost => _isAttemptingHost.Val;
	public bool IsConnectedHost => _isConnectedHost.Val;
	public bool IsAttemptingClient => _isAttemptingClient.Val;
	public bool IsConnectedClient => _isConnectedClient.Val;
	public bool IsInAnyState => _isInAnyState.Val;
	public Lobby? CurrentLobby => _currentLobby.Val;

	public async void StartHost()
	{
		if (_netManager.IsHost || _netManager.IsClient || _currentLobby.Val != null)
		{
			Debug.LogError("Already in a network state, we shouldn't be trying to host", this);
			return;
		}
		if (!_netManager.StartHost())
		{
			Debug.LogError("Failed to start host server", this);
			_toastManager.Show("Failed to start hosting");
			return;
		}

		_isNetManagerHosted.Val = true;

		if (!_steam) return;

		// Start to create a steam lobby
		var lobbyIndex = _validLobbyIndex;
		var newLobby = await SteamMatchmaking.CreateLobbyAsync(MAX_PLAYERS);

		// Scrap lobby if we disconnected while creating it
		if (lobbyIndex != _validLobbyIndex)
		{
			Debug.LogWarning("Received lobby join result but it was for an old lobby, ignoring", this);
			newLobby?.Leave();
			return;
		}

		if (newLobby == null)
		{
			Debug.LogWarning("Failed to create lobby", this);
			_toastManager.Show("Failed to create lobby");
			Disconnect();
			return;
		}

		newLobby?.SetFriendsOnly();
		newLobby?.SetJoinable(true);
		newLobby?.SetData("name", $"{SteamClient.Name}'s Lobby");
		newLobby?.SetData("game", SteamManager.SteamAppId.ToString());
		newLobby?.SetData("isRunning", $"{false}");
		_currentLobby.Val = newLobby.Value;
	}

	public void Disconnect()
	{
		bool wasClient = _isNetManagerClientConnected.Val;

		_currentLobby.Val?.Leave();
		_currentLobby.Val = null;
		_netManager.Shutdown();

		_isNetManagerHosted.Val = false;
		_isNetManagerClientAttempting.Val = false;
		_isNetManagerClientConnected.Val = false;
		_isClientAttemptingToJoinLobby.Val = false;

		_validLobbyIndex += 1; // Increment this to invalidate any lobbies we are currently getting

		// Otherwise, no events are called for this
		if (wasClient)
		{
			OnVoluntaryClientDisconnection.Invoke();
		}
	}

	private void OnClientConnected(ulong clientId)
	{
		if (clientId == NetworkManager.ServerClientId)
		{
			// We just started hosting
			_isNetManagerHosted.Val = true;
		}
		else if (clientId == _netManager.LocalClientId)
		{
			// We ourselves just connected as a client
			_isNetManagerClientConnected.Val = true;
			_isNetManagerClientAttempting.Val = false;
		}
	}

	private void OnClientDisconnected(ulong clientId)
	{
		if (_isNetManagerHosted.Val)
		{
			if (clientId == NetworkManager.ServerClientId)
			{
				Disconnect();
			}
		}
		else
		{
			// Otherwise, the server must have disconnected us
			// Note, this doesn't handle the case of voluntary disconnection
			Disconnect();
		}
	}

	private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
	{

		if (IsInAnyState)
		{
			Debug.LogWarning("Received lobby join request, but we're already doing something", this);
			_toastManager.Show("Can't join lobby - already in a lobby.");
			return;
		}

		if (!_steam)
		{
			Debug.LogError("Received lobby join request but we're not using steam transport, ignoring", this);
			return;
		}

		await AttemptJoinLobby(lobby);
	}

	public async void StartConnectToLobby(ulong lobbyId)
	{
		if (IsInAnyState)
		{
			Debug.LogError("Already in a network state, we shouldn't be trying to connect to a lobby", this);
			return;
		}

		if (!_steam)
		{
			Debug.LogError("Trying to connect to lobby but we're not using steam transport, ignoring", this);
			return;
		}

		var lobby = new Lobby(lobbyId);
		await AttemptJoinLobby(lobby);
	}

	private async Task AttemptJoinLobby(Lobby lobby)
	{
		var lobbyIndex = _validLobbyIndex;
		_isClientAttemptingToJoinLobby.Val = true;
		var result = await lobby.Join();
		_isClientAttemptingToJoinLobby.Val = false;

		// Scrap lobby if we disconnected while joining
		if (lobbyIndex != _validLobbyIndex)
		{
			Debug.LogWarning("Received lobby join result but it was for an old lobby, ignoring", this);
			lobby.Leave();
			return;
		}

		if (result != RoomEnter.Success)
		{
			_toastManager.Show($"Failed to connect to lobby: {result}");
			Debug.LogWarning($"Couldn't enter the lobby, {result}", this);
			Disconnect();
			return;
		}

		_currentLobby.Val = lobby;
		_steamTransport.targetSteamId = lobby.Owner.Id;
		_netManager.StartClient();
	}
}