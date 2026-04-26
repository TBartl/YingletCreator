using Reactivity;
using System.Linq;
using UnityEngine;

public class ReflectPartyMemberHUDObjects : ReactiveBehaviour
{
	[SerializeField] GameObject _partyMemberPrefab;

	private Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	EnumerableDictReflector<GameObject, GameObject> _dictReflector;

	void Start()
	{
		_dictReflector = new EnumerableDictReflector<GameObject, GameObject>(Added, Removed);
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>();

		// Destroy existing children (there for mock purposes)
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		AddReflector(Reflect);
	}

	private GameObject Added(GameObject characterGameObject)
	{
		using var disabler = _partyMemberPrefab.TemporarilyDisable();
		var hudGameObject = Instantiate(_partyMemberPrefab, transform);
		hudGameObject.GetComponentSafe<IWriteablePartyMemberHUDReference>().SetCharacterGameObject(characterGameObject);
		hudGameObject.SetActive(true);
		return hudGameObject;
	}

	private void Removed(GameObject hudGameObject)
	{
		Destroy(hudGameObject);
	}


	private void Reflect()
	{
		var characterManager = _expeditionCharacterManager.Val;
		if (characterManager == null)
		{
			_dictReflector.Enumerate(Enumerable.Empty<GameObject>());
			return;
		}
		_dictReflector.Enumerate(characterManager.Characters.Select(c => c.GameObject));
	}
}
