using Character.Creator;
using Reactivity;

public class CurrentCharacterHUD : ReactiveBehaviour, IPartyMemberHUDReference, ICachedYingletReference, IClassReference, IInitializable, ISelectable
{
	private Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	Computed<bool> _selected;
	Computed<ClassId> _class;
	Computed<ICharacterRoot> _character;


	Computed<SerializableCustomizationData> _cachedData;
	public SerializableCustomizationData CachedData => _cachedData.Val;

	public ClassId Class => _class.Val;

	public IReadOnlyObservable<bool> Selected => _selected;

	public ICharacterRoot Character => _character.Val;
	public IReadOnlyObservable<ICharacterRoot> CharacterObservable => _character;

	public void Initialize()
	{
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>();
		_character = CreateComputed(() => _expeditionCharacterManager.Val?.ActiveCharacter?.Val?.Root);
		_selected = CreateComputed(ComputeSelected);
		_class = CreateComputed(ComputeClass);
		_cachedData = CreateComputed(ComputeCustomizationData);
	}
	private bool ComputeSelected()
	{
		bool isSelected = Character != null;
		return isSelected;
	}
	private ClassId ComputeClass()
	{
		var character = Character;
		if (character == null) return null;
		var classRepo = character.GetComponentInChildrenSafe<IClassReference>();
		return classRepo.Class;
	}
	private SerializableCustomizationData ComputeCustomizationData()
	{
		var character = Character;
		if (character == null) return null;
		var customizationDataRepo = character.GetComponentInChildrenSafe<IGameCharacterDataRepository>();
		return customizationDataRepo.LastSerializedData.Val;
	}
}