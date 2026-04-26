using Character.Creator;
using Reactivity;
using UnityEngine;

public interface IPartyMemberHUDReference
{
	GameObject CharacterGameObject { get; }
	IReadOnlyObservable<GameObject> CharacterGameObjectObservable { get; }
}

public interface IWriteablePartyMemberHUDReference : IPartyMemberHUDReference
{
	void SetCharacterGameObject(GameObject characterGameObject);
}

public class PartyMemberHUDReference : ReactiveBehaviour, IWriteablePartyMemberHUDReference, ICachedYingletReference, IClassReference, IInitializable, ISelectable
{
	private IActiveCharacterProvider _activeCharacterProvider;
	Observable<GameObject> _characterGameObject = new Observable<GameObject>();
	Computed<bool> _selected;
	Computed<ClassId> _class;

	public GameObject CharacterGameObject => _characterGameObject.Val;
	public IReadOnlyObservable<GameObject> CharacterGameObjectObservable => _characterGameObject;

	Computed<SerializableCustomizationData> _cachedData;
	public SerializableCustomizationData CachedData => _cachedData.Val;

	public ClassId Class => _class.Val;

	public void SetCharacterGameObject(GameObject characterGameObject)
	{
		_characterGameObject.Val = characterGameObject;
	}

	public IReadOnlyObservable<bool> Selected => _selected;


	public void Initialize()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_selected = CreateComputed(ComputeSelected);
		_class = CreateComputed(ComputeClass);
		_cachedData = CreateComputed(ComputeCustomizationData);
	}

	private bool ComputeSelected()
	{
		return _activeCharacterProvider.ActiveCharacter.Val == _characterGameObject.Val;
	}

	private ClassId ComputeClass()
	{
		var characterGameObject = CharacterGameObject;
		if (characterGameObject == null) return null;
		var classRepo = characterGameObject.GetComponentInChildrenSafe<IClassReference>();
		return classRepo.Class;
	}

	private SerializableCustomizationData ComputeCustomizationData()
	{
		var characterGameObject = CharacterGameObject;
		if (characterGameObject == null) return null;
		var customizationDataRepo = characterGameObject.GetComponentInChildrenSafe<IGameCharacterDataRepository>();
		return customizationDataRepo.LastSerializedData.Val;
	}
}
