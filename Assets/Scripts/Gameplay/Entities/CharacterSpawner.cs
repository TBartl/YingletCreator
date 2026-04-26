using Reactivity;
using System;
using UnityEngine;

public enum CharacterPrefabType
{
	Lobby,
	Expedition
}

public interface ICharacterSpawner
{
	GameObject SpawnCharacter(Action<GameObject> beforeEnable, CharacterPrefabType prefabType);
}

public class CharacterSpawner : ReactiveBehaviour, ICharacterSpawner
{
	[SerializeField] GameObject _lobbyCharacterPrefab;
	[SerializeField] GameObject _expeditionCharacterPrefab;

	public GameObject SpawnCharacter(Action<GameObject> beforeEnable, CharacterPrefabType prefabType)
	{
		using var _ = new ReactivityTrackingSuspender(); // When we spawn an object, we don't want to listen on anything it's doing
		var prefabToUse = prefabType switch
		{
			CharacterPrefabType.Lobby => _lobbyCharacterPrefab,
			CharacterPrefabType.Expedition => _expeditionCharacterPrefab,
			_ => throw new ArgumentOutOfRangeException(nameof(prefabType), prefabType, null)
		};
		using var disabler = prefabToUse.TemporarilyDisable();
		var character = Instantiate(prefabToUse);

		beforeEnable(character);
		character.SetActive(true);
		return character;
	}
}
