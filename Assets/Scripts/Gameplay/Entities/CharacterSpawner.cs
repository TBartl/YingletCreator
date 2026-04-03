using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICharacterSpawner
{
	GameObject MyCharacter { get; }
}

public class CharacterSpawner : ReactiveBehaviour, ICharacterSpawner
{
	Observable<GameObject> _myCharacter = new Observable<GameObject>(null);

	[SerializeField] GameObject _characterPrefab;

	Dictionary<ulong, GameObject> _characters = new Dictionary<ulong, GameObject>();
	private INetClientTracker _netClientTracker;

	public GameObject MyCharacter => _myCharacter.Val;

	private void Start()
	{
		_netClientTracker = Singletons.GetSingleton<INetClientTracker>();
		AddReflector(ReflectYingletPerClient);
	}

	private void ReflectYingletPerClient()
	{
		var clientIds = new HashSet<ulong>(_netClientTracker.ClientIDs);
		var myClientId = _netClientTracker.LocalClientID;

		using var disabler = new ReactivityTrackingSuspender();

		foreach (var clientId in clientIds)
		{
			if (!_characters.ContainsKey(clientId))
			{
				var yinglet = Instantiate(_characterPrefab);
				_characters[clientId] = yinglet;
			}
		}

		var yingletsToRemove = _characters.Keys.Where(clientId => !clientIds.Contains(clientId)).ToList();
		foreach (var clientId in yingletsToRemove)
		{
			Destroy(_characters[clientId]);
			_characters.Remove(clientId);
		}

		if (_characters.TryGetValue(myClientId, out var myYinglet))
		{
			_myCharacter.Val = myYinglet;
		}
		else
		{
			_myCharacter.Val = null;
			Debug.LogWarning("My client ID does not have a character assigned.");
		}
	}
}
