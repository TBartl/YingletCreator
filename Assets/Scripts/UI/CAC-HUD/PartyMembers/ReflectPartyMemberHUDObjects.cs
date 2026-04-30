using Reactivity;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReflectPartyMemberHUDObjects : ReactiveBehaviour
{
	[SerializeField] GameObject _partyMemberPrefab;

	private Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	EnumerableDictReflector<ICharacterRoot, GameObject> _dictReflector;

	void Start()
	{
		_dictReflector = new EnumerableDictReflector<ICharacterRoot, GameObject>(Added, Removed);
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>();

		// Destroy existing children (there for mock purposes)
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		AddReflector(Reflect);
	}

	private GameObject Added(ICharacterRoot character)
	{
		using var disabler = _partyMemberPrefab.TemporarilyDisable();
		var hudGameObject = Instantiate(_partyMemberPrefab, transform);
		hudGameObject.GetComponentSafe<IWriteablePartyMemberHUDReference>().SetCharacter(character);
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
			_dictReflector.Enumerate(Enumerable.Empty<ICharacterRoot>());
			return;
		}
		_dictReflector.Enumerate(characterManager.Characters.Select(c => c.Root));

		// I don't know why this is necessary, but without it this seems to be bugging out on occasion
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
	}
}
