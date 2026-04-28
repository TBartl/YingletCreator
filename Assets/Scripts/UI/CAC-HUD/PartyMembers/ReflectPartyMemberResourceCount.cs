using Reactivity;
using TMPro;
using UnityEngine;

public class ReflectPartyMemberResourceCount : ReactiveBehaviour
{
	[SerializeField] CharacterResourceType _resourceType;

	private TMP_Text _text;
	private IPartyMemberHUDReference _reference;
	Computed<ICharacterResources> _dataRepo;

	void Start()
	{
		_text = this.GetComponent<TMP_Text>();
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_dataRepo = CreateComputed(ComputeDataRepo);
		AddReflector(ReflectResourceCount);
	}

	private ICharacterResources ComputeDataRepo()
	{
		var characterGameObject = _reference.CharacterGameObject;
		if (characterGameObject == null) return null;
		return characterGameObject.GetComponentInChildrenSafe<ICharacterResources>();
	}

	private void ReflectResourceCount()
	{
		_text.text = $"x{_dataRepo.Val?.GetResource(_resourceType).ToString() ?? "0"}";
	}
}