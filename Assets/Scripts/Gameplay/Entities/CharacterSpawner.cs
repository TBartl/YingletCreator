using Reactivity;
using System;
using UnityEngine;

public interface ICharacterSpawner
{
	GameObject SpawnCharacter(ulong connectionId);
	GameObject SpawnCharacter(ulong connectionId, Action<GameObject> beforeEnable);
}

public class CharacterSpawner : ReactiveBehaviour, ICharacterSpawner
{
	[SerializeField] GameObject _characterPrefab;

	public GameObject SpawnCharacter(ulong connectionId)
	{
		return SpawnCharacter(connectionId, _ => { });
	}

	public GameObject SpawnCharacter(ulong connectionId, Action<GameObject> beforeEnable)
	{
		using var _ = new ReactivityTrackingSuspender(); // When we spawn an object, we don't want to listen on anything it's doing
		using var disabler = _characterPrefab.TemporarilyDisable();
		var character = Instantiate(_characterPrefab);
		character.GetComponent<IPlayerIdentity>().ConnectionId = connectionId;
		beforeEnable(character);
		character.SetActive(true);
		return character;
	}
}
