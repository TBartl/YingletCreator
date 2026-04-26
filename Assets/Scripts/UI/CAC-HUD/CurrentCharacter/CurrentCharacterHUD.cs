using Character.Creator;
using Reactivity;
using UnityEngine;

public class CurrentCharacterHUD : ReactiveBehaviour, IPartyMemberHUDReference, ICachedYingletReference, IClassReference, IInitializable, ISelectable
{
	private Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	Computed<bool> _selected;
	Computed<ClassId> _class;
	Computed<GameObject> _characterGameObject;


	Observable<SerializableCustomizationData> _cachedData = new Observable<SerializableCustomizationData>();
	public SerializableCustomizationData CachedData => _cachedData.Val;

	public ClassId Class => _class.Val;

	public IReadOnlyObservable<bool> Selected => _selected;

	public GameObject CharacterGameObject => _characterGameObject.Val;
	public IReadOnlyObservable<GameObject> CharacterGameObjectObservable => _characterGameObject;

	public void Initialize()
	{
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>();
		_characterGameObject = CreateComputed(() => _expeditionCharacterManager.Val?.ActiveCharacter?.Val?.GameObject);
		_selected = CreateComputed(ComputeSelected);
		_class = CreateComputed(ComputeClass);
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

	private bool ComputeSelected()
	{
		bool isSelected = CharacterGameObject != null;
		return isSelected;
	}
	private ClassId ComputeClass()
	{
		var characterGameObject = CharacterGameObject;
		if (characterGameObject == null) return null;
		var classRepo = characterGameObject.GetComponentInChildrenSafe<IClassReference>();
		return classRepo.Class;
	}
}