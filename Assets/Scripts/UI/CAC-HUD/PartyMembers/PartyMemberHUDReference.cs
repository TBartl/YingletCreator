using Character.Creator;
using Reactivity;
using UnityEngine;

public interface IPartyMemberHUDReference
{
	GameObject CharacterGameObject { get; set; }
}

public class PartyMemberHUDReference : ReactiveBehaviour, IPartyMemberHUDReference, ICachedYingletReference, IClassReference, IInitializable, ISelectable
{
	private IActiveCharacterProvider _activeCharacterProvider;
	Observable<GameObject> _characterGameObject = new Observable<GameObject>();
	Computed<bool> _selected;
	Computed<ClassId> _class;

	public GameObject CharacterGameObject
	{
		get => _characterGameObject.Val;
		set => _characterGameObject.Val = value;
	}

	Observable<SerializableCustomizationData> _cachedData = new Observable<SerializableCustomizationData>();
	public SerializableCustomizationData CachedData => _cachedData.Val;

	public ClassId Class => _class.Val;

	public IReadOnlyObservable<bool> Selected => throw new System.NotImplementedException();

	public void Initialize()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_selected = CreateComputed(ComputeSelected);
		_class = CreateComputed(ComputeClass);

	}

	private bool ComputeSelected()
	{
		return _activeCharacterProvider.ActiveCharacter.Val == _characterGameObject.Val;
	}

	private void Start()
	{
		// This is currently just statically defined
		// It might be better for the character GameObject to provide this since it will know better when it needs to be updated
		CreateInitialPortrait();
	}

	void CreateInitialPortrait()
	{
		var characterGameObject = CharacterGameObject;
		if (characterGameObject == null) return;
		var dataRepo = characterGameObject.GetComponentInChildrenSafe<ICustomizationDataRepository>();
		_cachedData.Val = new SerializableCustomizationData(dataRepo.CustomizationData);
	}

	private ClassId ComputeClass()
	{
		var characterGameObject = CharacterGameObject;
		if (characterGameObject == null) return null;
		var classRepo = characterGameObject.GetComponentInChildrenSafe<IClassReference>();
		return classRepo.Class;
	}

}
