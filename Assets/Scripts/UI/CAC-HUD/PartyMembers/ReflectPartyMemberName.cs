using Character.Creator;
using Reactivity;
using TMPro;

public class ReflectPartyMemberName : ReactiveBehaviour
{
	private TMP_Text _text;
	private IPartyMemberHUDReference _reference;
	Computed<ICustomizationDataRepository> _dataRepo;

	void Start()
	{
		_text = this.GetComponent<TMP_Text>();
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_dataRepo = CreateComputed(ComputeDataRepo);
		AddReflector(ReflectName);
	}

	private ICustomizationDataRepository ComputeDataRepo()
	{
		var character = _reference.Character;
		if (character == null) return null;
		return character.GetComponentInChildrenSafe<ICustomizationDataRepository>();
	}

	private void ReflectName()
	{
		_text.text = _dataRepo.Val?.CustomizationData?.Name?.Val ?? "Unknown";
	}
}