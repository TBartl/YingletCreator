using Reactivity;

public class ReflectEnergyBlipActive : ReactiveBehaviour
{
	private ICommonGameplayAssets _assets;
	private int _parentSiblingIndex;
	private IPartyMemberHUDReference _reference;
	private Computed<ICharacterResources> _characterResources;
	private Computed<int> _resourceCount;

	void Start()
	{
		_assets = Singletons.GetSingleton<ICommonGameplayAssets>();
		_parentSiblingIndex = this.transform.parent.GetSiblingIndex();
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_characterResources = CreateComputed(ComputeCharacterResources);
		_resourceCount = CreateComputed(() => _characterResources.Val?.GetResource(_assets.ResourceEnergy) ?? 0);
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
