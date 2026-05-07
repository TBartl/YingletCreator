using Reactivity;
using TMPro;
using UnityEngine;

public class ReflectPartyMemberResourceCount : ReactiveBehaviour
{
	[SerializeField] AssetReferenceT<CharacterResourceId> _resource;

	private TMP_Text _text;
	private IPartyMemberHUDReference _reference;
	Computed<ICharacterResources> _dataRepo;

	void Start()
	{
		_text = this.GetComponent<TMP_Text>();
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_dataRepo = CreateComputed(ComputeCharacterResources);
		AddReflector(ReflectResourceCount);
	}

	private ICharacterResources ComputeCharacterResources()
	{
		var character = _reference.Character;
		if (character == null) return null;
		return character.GetComponentInChildrenSafe<ICharacterResources>();
	}

	private void ReflectResourceCount()
	{
		_text.text = $"x{_dataRepo.Val?.GetResource(_resource.LoadSync()).ToString() ?? "0"}";
	}
}