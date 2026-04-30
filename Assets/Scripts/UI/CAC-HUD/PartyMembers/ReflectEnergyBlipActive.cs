using Reactivity;

public class ReflectEnergyBlipActive : ReactiveBehaviour
{
	private int _parentSiblingIndex;
	private IPartyMemberHUDReference _reference;
	private Computed<ICharacterResources> _characterResources;
	private Computed<int> _resourceCount;

	void Start()
	{
		_parentSiblingIndex = this.transform.parent.GetSiblingIndex();
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_characterResources = CreateComputed(ComputeCharacterResources);
		_resourceCount = CreateComputed(() => _characterResources.Val?.GetResource(CharacterResourceType.Energy) ?? 0);
		AddReflector(Reflect);
	}


	private ICharacterResources ComputeCharacterResources()
	{
		var character = _reference.Character;
		if (character == null) return null;
		return character.GetComponentInChildrenSafe<ICharacterResources>();
	}
	void Reflect()
	{
		this.gameObject.SetActive(_resourceCount.Val > _parentSiblingIndex);
	}
}
