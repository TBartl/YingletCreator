using Reactivity;
using Reactivity.Implementation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface ILobbyCharacterManager
{
	IReadOnlyDictionary<ulong, GameObject> Characters { get; }
}

public class LobbyCharacterManager : ReactiveBehaviour, ILobbyCharacterManager
{
	[SerializeField] Transform _spawnPoint;
	[SerializeField] Transform _characterParent;

	Dictionary<ulong, GameObject> _characters = new Dictionary<ulong, GameObject>();
	Notifier _dictNotifier = new(); // Easier to do it like this than using a reactive dictionary since we read from it a lot
	private INetClientTracker _netClientTracker;
	private ICharacterSpawner _characterSpawner;
	private ulong _lastLocalClientId = 0;


	public IReadOnlyDictionary<ulong, GameObject> Characters
	{
		get
		{
			_dictNotifier.Track();
			return _characters;
		}
	}

	private void Start()
	{
		_netClientTracker = Singletons.GetSingleton<INetClientTracker>();
		_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		AddReflector(ReflectYingletPerClient);
	}

	private void ReflectYingletPerClient()
	{
		var clientData = _netClientTracker.Data;
		var myClientId = clientData.LocalClientID;
		var clientIds = clientData.ClientIDs.ToList();

		using var disabler = new ReactivityTrackingSuspender();

		// Debug.Log($"Reflecting characters for clients: {string.Join(", ", clientIds)}. Old client ID: {_lastLocalClientId}. New client ID: {myClientId}");

		// Handle local client ID change
		if (myClientId != _lastLocalClientId)
		{
			// If the new client ID already has a character, delete it
			if (_characters.TryGetValue(myClientId, out var conflictingCharacter))
			{
				Destroy(conflictingCharacter);
				_characters.Remove(myClientId);
			}

			_characters[myClientId] = _characters[_lastLocalClientId];
			_characters.Remove(_lastLocalClientId);

			_lastLocalClientId = myClientId;
		}

		// Ensure the current player has a character
		if (!_characters.ContainsKey(myClientId))
		{
			_characters[myClientId] = CreateNewCharacterObject(myClientId);
		}

		// Update the player's character reference
		if (_characters.TryGetValue(myClientId, out var myCharacter))
		{
			myCharacter.GetComponent<IPlayerIdentity>().ConnectionId = myClientId;
		}

		// Create or update characters for other clients
		foreach (var clientId in clientIds.Where(id => id != myClientId))
		{
			if (!_characters.ContainsKey(clientId))
			{
				_characters[clientId] = CreateNewCharacterObject(clientId);
			}
		}

		// Destroy characters for clients that are no longer connected
		var charactersToRemove = _characters.Keys.Where(clientId => !clientIds.Contains(clientId)).ToList();
		foreach (var clientId in charactersToRemove)
		{
			Destroy(_characters[clientId]);
			_characters.Remove(clientId);
		}

		_dictNotifier.Dirty();
	}

	GameObject CreateNewCharacterObject(ulong clientId)
	{
		var character = _characterSpawner.SpawnCharacter(clientId, character =>
		{
			character.transform.position = _spawnPoint.position;
			character.transform.SetParent(_characterParent);
			character.GetComponent<IPlayerIdentity>().ConnectionId = clientId;
		});
		return character;
	}
}

