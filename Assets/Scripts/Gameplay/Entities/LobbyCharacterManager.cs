using Networking;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public sealed class LobbyCharacter
{
	public LobbyCharacter(GameObject gameObject)
	{
		GameObject = gameObject;
		_identity = gameObject.GetComponentInChildrenSafe<IWriteableCharacterIdentity>();
	}

	public GameObject GameObject;
	private IWriteableCharacterIdentity _identity;
	public IWriteableCharacterIdentity Identity => _identity;
}

public interface ILobbyCharacterManager
{
	IEnumerable<LobbyCharacter> Characters { get; }
}

public class LobbyCharacterManager : MonoBehaviour, ILobbyCharacterManager, IInitializable
{
	[SerializeField] Transform _spawnPoint;
	[SerializeField] Transform _characterParent;

	ObservableList<LobbyCharacter> _characters = new ObservableList<LobbyCharacter>();
	private ICharacterSpawner _characterSpawner;
	private INetStateReader _netState;
	private INetEventBus _netEventBus;
	private INetIdentityProvider _netIdentityProvider;
	LobbyCharacter _myCharacter = null;

	public IEnumerable<LobbyCharacter> Characters => _characters;

	public void Initialize()
	{
		_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		_netState = Singletons.GetSingleton<INetStateReader>();
		_netEventBus = Singletons.GetSingleton<INetEventBus>();
		_netIdentityProvider = Singletons.GetSingleton<INetIdentityProvider>();

		_netState.OnLocalDisconnected += NetState_OnLocalDisconnected;
		_netState.OnClientConnectedToUs += NetState_OnClientConnectedToUs;
		_netState.OnClientDisconnectedFromUs += NetState_OnClientDisconnectedFromUs;

		_netEventBus.Subscribe<Message_CreateLobbyCharacter>(EventBus_OnCreateLobbyCharacter);
		_netEventBus.Subscribe<Message_RemoveLobbyCharacter>(EventBus_OnRemoveLobbyCharacter);
	}

	private void OnDestroy()
	{
		_netState.OnLocalDisconnected -= NetState_OnLocalDisconnected;
		_netState.OnClientConnectedToUs -= NetState_OnClientConnectedToUs;
		_netState.OnClientDisconnectedFromUs -= NetState_OnClientDisconnectedFromUs;

		_netEventBus.Unsubscribe<Message_CreateLobbyCharacter>(EventBus_OnCreateLobbyCharacter);
		_netEventBus.Unsubscribe<Message_RemoveLobbyCharacter>(EventBus_OnRemoveLobbyCharacter);
	}


	void Start()
	{
		_myCharacter = CreateNewCharacterObject(0);
	}

	private void NetState_OnLocalDisconnected()
	{
		// 1. Remove all characters other than ourself
		var charactersToRemove = _characters.Where(c => c != _myCharacter).ToList();
		foreach (var character in charactersToRemove)
		{
			_characters.Remove(character);
			Destroy(character.GameObject);
		}

		// 2. Re-assign our character to owner 0 (our new id)
		if (_myCharacter != null)
		{
			_myCharacter.Identity.SetOwner(0);
		}
	}

	private void NetState_OnClientConnectedToUs(ulong clientId)
	{
		// 1. Notify connecting client of all existing characters
		foreach (var character in _characters)
		{
			var message = new Message_CreateLobbyCharacter(character.Identity.OwnerClientId, character.Identity.NetId);
			_netEventBus.SendToOne(message, clientId);
		}

		// 2. Notify all clients to create the new character
		_netEventBus.SendToAll(new Message_CreateLobbyCharacter(clientId, _netIdentityProvider.GetNextId()));
	}

	private void NetState_OnClientDisconnectedFromUs(ulong clientId)
	{
		// Notify all clients to remove the character
		var character = _characters.FirstOrDefault(c => c.Identity.OwnerClientId == clientId);
		if (character != null)
		{
			_netEventBus.SendToAll(new Message_RemoveLobbyCharacter(character.Identity.NetId));
		}
	}



	private void EventBus_OnRemoveLobbyCharacter(Message_RemoveLobbyCharacter message, ulong senderClientId)
	{
		var character = _characters.FirstOrDefault(c => c.Identity.NetId == message.NetId);
		if (character != null)
		{
			_characters.Remove(character);
			Destroy(character.GameObject);

			if (character == _myCharacter)
			{
				_myCharacter = null;
			}
		}
	}

	private void EventBus_OnCreateLobbyCharacter(Message_CreateLobbyCharacter message, ulong senderClientId)
	{
		if (message.ClientId == _netState.LocalClientID)
		{
			// This is telling us about our own character
			if (_myCharacter != null)
			{
				// If we have one, let's just re-assign it
				_myCharacter.Identity.ForceIdentity(message.NetId);
				_myCharacter.Identity.SetOwner(message.ClientId);
			}
			else
			{
				// Otherwise, we need to re-make it
				_myCharacter = CreateNewCharacterObject(message.ClientId, message.NetId);
			}
		}
		else
		{
			// This is telling us about someone else's character, so we just create it
			CreateNewCharacterObject(message.ClientId, message.NetId);
		}
	}

	LobbyCharacter CreateNewCharacterObject(ulong clientId, ulong? netId = null)
	{
		var character = _characterSpawner.SpawnCharacter(character =>
		{
			character.transform.position = _spawnPoint.position;
			character.transform.SetParent(_characterParent);

			var identity = character.GetComponentSafe<IWriteableCharacterIdentity>();
			if (netId != null)
			{
				identity.ForceIdentity(netId.Value);
			}

			identity.SetOwner(clientId);
		});
		var lobbyCharacter = new LobbyCharacter(character);
		_characters.Add(lobbyCharacter);
		return lobbyCharacter;
	}

}


struct Message_CreateLobbyCharacter : INetMessage
{
	public ulong ClientId;
	public ulong NetId;
	public Message_CreateLobbyCharacter(ulong clientId, ulong netId)
	{
		ClientId = clientId;
		NetId = netId;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref ClientId);
		serializer.SerializeValue(ref NetId);
	}
}

struct Message_RemoveLobbyCharacter : INetMessage
{
	public ulong NetId;
	public Message_RemoveLobbyCharacter(ulong netId)
	{
		NetId = netId;
	}
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
	}
}