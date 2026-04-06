using Netcode.Transports.Facepunch;
using Reactivity;
using Steamworks;
using Steamworks.Data;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The layer system sits outside of the Steamworks transport
/// This class abstracts that, even for non-Steamworks transports
/// Adapted from https://github.com/MrRobinOfficial/Guide-UnitySteamNetcodeGameObjects/blob/49aa57127dec170d970434dc9be9254b10c66dca/Code/Framwork/NW_NetworkManager.cs
/// </summary>
public interface INetLobbyManager
{
	public void StartHostWithLobby();

	Lobby? CurrentLobby { get; }
}

public class NetLobbyManager : MonoBehaviour, INetLobbyManager
{
	public const int MaxSteamLobbyPlayers = 64;

	///// NETWORK CALLBACKS ///
	//public static event UnityAction OnClientReadied;
	//public static event UnityAction OnClientDisconnectRequested;
	//public static event UnityAction<ulong, int> OnClientSceneChanged;
	//public static event UnityAction<ConnectStatus> OnConnectionCompleted;
	//public static event UnityAction<ConnectStatus> OnDisconnectReceived;

	///// STEAM CALLBACKS ///
	//public static event UnityAction OnLobbyCreatedEvent;
	//public static event UnityAction OnLobbyDataChangedEvent;
	//public static event UnityAction<string> OnLobbyChatMessageDeliveredEvent;
	//public static event UnityAction<Friend, string> OnLobbyChatMessageReceivedEvent;

	//public static event UnityAction<Friend> OnMemberDataChangedEvent;
	//public static event UnityAction<Friend> OnMemberJoinedEvent;
	//public static event UnityAction<Friend> OnMemberLeftEvent;
	//public static event UnityAction<Friend, Lobby> OnMemberInviteReceivedEvent;
	//public static event UnityAction<Friend, Friend> OnMemberKickedEvent;
	//public static event UnityAction<Friend, Friend> OnMemberBannedEvent;

	private Observable<Lobby?> _currentLobby = new();
	public Lobby? CurrentLobby => _currentLobby.Val;

	private NetworkManager _netManager;
	private CustomFacepunchTransport _steamTransport;
	private INetStateProvider _netStateProvider;

	private void Start()
	{
		_netManager = NetworkManager.Singleton;
		_steamTransport = _netManager.NetworkConfig.NetworkTransport as CustomFacepunchTransport;
		_netStateProvider = this.GetComponent<INetStateProvider>();
		_netStateProvider.OnDisconnect += OnDisconnected;

		SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;

		//SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
		//SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
		//SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
		//SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;
		//SteamMatchmaking.OnLobbyMemberKicked += OnLobbyMemberKicked;
		//SteamMatchmaking.OnLobbyMemberBanned += OnLobbyMemberBanned;
		//SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
		//SteamMatchmaking.OnChatMessage += OnChatMessage;
		//SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChanged;
		//SteamMatchmaking.OnLobbyMemberDataChanged += OnLobbyMemberDataChanged;

		//SteamFriends.OnGameRichPresenceJoinRequested += OnGameRichPresenceJoinRequested;

		//_netManager.OnServerStarted += OnConnectedCallback;
		//_netManager.OnClientDisconnectCallback += OnClientDisconnected;
	}

	private void OnDestroy()
	{
		_netStateProvider.OnDisconnect -= OnDisconnected;
		SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
	}

	private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
	{
		if (_steamTransport == null)
		{
			Debug.LogError("Received lobby join request but transport is not a CustomFacepunchTransport", this);
			return;
		}
		_currentLobby.Val = lobby;
		var result = await lobby.Join();
		if (result != RoomEnter.Success)
		{
			Debug.LogError($"Couldn't enter the lobby, {result}", this);
			_currentLobby.Val = null;
			return;
		}

		_steamTransport.targetSteamId = lobby.Owner.Id;
		_netManager.StartClient();
	}

	private void OnDisconnected()
	{
		CurrentLobby?.Leave();
		_currentLobby.Val = null;
	}

	private void OnApplicationQuit() => CurrentLobby?.Leave();

	bool IsSteamRunning => SteamClient.IsValid;

	private async Task CreateLobbyAsync(LobbyConfig config)
	{
		var newLobby = await SteamMatchmaking.CreateLobbyAsync(config.maxMembers);
		if (newLobby == null)
		{
			Debug.LogError("Failed to create lobby", this);
			return;
		}

		newLobby?.SetVisibility(config.visibility);
		newLobby?.SetJoinable(config.joinable);
		newLobby?.SetData("name", config.name);
		newLobby?.SetData("game", SteamManager.SteamAppId.ToString());
		newLobby?.SetData("isRunning", $"{false}");
		_currentLobby.Val = newLobby.Value;
	}

	public void StartHostWithLobby()
	{
		if (!_netManager.StartHost())
		{
			Debug.LogError("Failed to start host server", this);
			return;
		}

		// Only create a lobby if steam is running
		// For other transports
		if (IsSteamRunning)
		{
			var lobbyConfig = new LobbyConfig
			{
				name = $"{SteamClient.Name}'s Lobby",
				joinable = true,
				visibility = LobbyVisibility.Invisible,
				maxMembers = MaxSteamLobbyPlayers,
			};
			CreateLobbyAsync(lobbyConfig).FireAndForgetWithLogging();
		}
	}
}

public enum ConnectStatus : byte
{
	Undefined,
	Success,
	ServerFull,
	GameInProgress,
	LoggedInAgain,
	UserRequestedDisconnect,
	GenericDisconnect,
	KickDisconnect,
	BanDisconnect,
}

public enum LobbyVisibility : byte
{
	Public,
	Private,
	FriendsOnly,
	Invisible,
}

[System.Serializable]
public struct LobbyConfig
{
	public string name;
	public bool joinable;
	public LobbyVisibility visibility;
	public byte maxMembers;
}

public static class NW_SteamExtensions
{
	public static void SetVisibility(this Lobby lobby, LobbyVisibility visibility)
	{
		switch (visibility)
		{
			case LobbyVisibility.Public: lobby.SetPublic(); break;
			case LobbyVisibility.Private: lobby.SetPrivate(); break;
			case LobbyVisibility.FriendsOnly: lobby.SetFriendsOnly(); break;
			case LobbyVisibility.Invisible: lobby.SetInvisible(); break;
		}
	}

}