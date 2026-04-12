using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICharacterSpawner
{
	GameObject MyCharacter { get; }

	IReadOnlyDictionary<ulong, GameObject> Characters { get; }
}

public class CharacterSpawner : ReactiveBehaviour, ICharacterSpawner
{
	Observable<GameObject> _myCharacter = new Observable<GameObject>(null);

	[SerializeField] GameObject _characterPrefab;

	Dictionary<ulong, GameObject> _characters = new Dictionary<ulong, GameObject>();
	private INetClientTracker _netClientTracker;
	private ulong _lastLocalClientId = 0;

	public GameObject MyCharacter => _myCharacter.Val;

	public IReadOnlyDictionary<ulong, GameObject> Characters => _characters;

	private void Start()
	{
		_netClientTracker = Singletons.GetSingleton<INetClientTracker>();
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
			_myCharacter.Val = myCharacter;
			myCharacter.GetComponent<IPlayerIdentity>().ConnectionId = myClientId;
		}
		else
		{
			_myCharacter.Val = null;
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
	}

	GameObject CreateNewCharacterObject(ulong connectionId)
	{
		using var _ = new ReactivityTrackingSuspender(); // When we spawn an object, we don't want to listen on anything it's doing
		using var disabler = _characterPrefab.TemporarilyDisable();
		var character = Instantiate(_characterPrefab);
		character.GetComponent<IPlayerIdentity>().ConnectionId = connectionId;
		character.SetActive(true);
		return character;
	}
}
