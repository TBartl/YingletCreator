using Reactivity;
using System.Collections.Generic;
using UnityEngine;

public sealed class LobbyCharacter
{
	public LobbyCharacter(GameObject gameObject)
	{
		GameObject = gameObject;
		_identity = gameObject.GetComponentInChildren<IPlayerIdentity>();
	}

	public GameObject GameObject;
	public ulong ClientId => _identity.ConnectionId;
	private IPlayerIdentity _identity;
}

public interface ILobbyCharacterManager
{
	IEnumerable<LobbyCharacter> LobbyCharacters { get; }
}

public class LobbyCharacterManager : MonoBehaviour, ILobbyCharacterManager
{
	private ICharacterSpawner _characterSpawner;

	ObservableList<LobbyCharacter> _lobbyCharacters = new ObservableList<LobbyCharacter>();
	public IEnumerable<LobbyCharacter> LobbyCharacters => _lobbyCharacters;

	private void Start()
	{
		_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		var characterObject = _characterSpawner.SpawnCharacter(0);
		var lobbyCharacter = new LobbyCharacter(characterObject);
		_lobbyCharacters.Add(lobbyCharacter);
	}
}
