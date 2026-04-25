using Character.Creator;
using Reactivity;
using System.Linq;
using UnityEngine;

public interface IPartyMemberHUDReference
{
	GameObject CharacterGameObject { get; set; }
}

public class PartyMemberHUDReference : MonoBehaviour, IPartyMemberHUDReference, ICachedYingletReference, IClassReference
{
	Observable<GameObject> _characterGameObject = new Observable<GameObject>();

	public GameObject CharacterGameObject
	{
		get => _characterGameObject.Val;
		set => _characterGameObject.Val = value;
	}

	Observable<SerializableCustomizationData> _cachedData = new Observable<SerializableCustomizationData>();
	public SerializableCustomizationData CachedData => _cachedData.Val;

	public ClassId Class { get; private set; }

	private void Awake()
	{
		// This is currently just statically defined
		// It might be better for the character GameObject to provide this since it will know better when it needs to be updated
		CreateInitialPortrait();


		var siblingIndex = transform.GetSiblingIndex();
		// temp code
		Class = Singletons.GetSingleton<ICompositeResourceLoader>().LoadClasses().OrderBy(i => i.OrderIndex).ToArray()[siblingIndex];
	}

	void CreateInitialPortrait()
	{
		var characterGameObject = CharacterGameObject;
		if (characterGameObject == null) return;
		var dataRepo = characterGameObject.GetComponentInChildrenSafe<ICustomizationDataRepository>();
		_cachedData.Val = new SerializableCustomizationData(dataRepo.CustomizationData);
	}
}
