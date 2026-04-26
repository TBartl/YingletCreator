using Character.Creator;
using Networking;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public sealed class ExpeditionCharacter
{
	public ExpeditionCharacter(GameObject gameObject)
	{
		GameObject = gameObject;
		_identity = gameObject.GetComponentInChildrenSafe<ICharacterIdentity>();
	}

	public GameObject GameObject;
	public ulong ClientId => _identity.OwnerClientId;
	private ICharacterIdentity _identity;
}

public interface IExpeditionCharacterManager
{
	IReadOnlyObservable<ExpeditionCharacter> ActiveCharacter { get; }
	IEnumerable<ExpeditionCharacter> Characters { get; }

	void SetActiveCharacter(GameObject character);
}
public class ExpeditionCharacterManager : MonoBehaviour, IExpeditionCharacterManager
{
	[SerializeField] Transform[] _spawnPoints;

	Observable<ExpeditionCharacter> _activeCharacter = new Observable<ExpeditionCharacter>();
	ObservableList<ExpeditionCharacter> _characters = new ObservableList<ExpeditionCharacter>();
	private IExpeditionPlanningManager _expeditionPlanningManager;
	private ICharacterSpawner _characterSpawner;
	private ExpeditionRoot _root;
	private GameObject _parentObject;

	public IReadOnlyObservable<ExpeditionCharacter> ActiveCharacter => _activeCharacter;
	public IEnumerable<ExpeditionCharacter> Characters => _characters;

	void Start()
	{
		_expeditionPlanningManager = Singletons.GetSingleton<IExpeditionPlanningManager>();
		_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		_root = this.GetComponentInParent<ExpeditionRoot>();

		// Create root object
		_parentObject = new GameObject("Characters");
		_parentObject.transform.SetParent(_root.transform, false);

		// Create all the players
		for (int i = 0; i < _expeditionPlanningManager.CurrentParty.Count; i++)
		{
			var partyMember = _expeditionPlanningManager.CurrentParty[i];

			var gameObject = _characterSpawner.SpawnCharacter((gameObject) =>
			{
				gameObject.transform.SetParent(_parentObject.transform);
				gameObject.transform.position = GetNextSpawn().position;

				var identity = gameObject.GetComponentSafe<IWriteableCharacterIdentity>();
				identity.SetOwner(partyMember.ClientId);

				var dataRepo = gameObject.GetComponentInChildren<IForceableCustomizationDataRepository>();
				dataRepo.ForceCustomizationData(partyMember.CustomizationData);

				// temp code 
				var classId = Singletons.GetSingleton<ICompositeResourceLoader>().LoadClasses().OrderBy(i => i.OrderIndex).ToArray()[i];
				var classReference = gameObject.GetComponentInChildrenSafe<IWriteableClassReference>();
				classReference.SetClass(classId);
			}, CharacterPrefabType.Expedition);

			_characters.Add(new ExpeditionCharacter(gameObject));
		}
		_activeCharacter.Val = _characters.FirstOrDefault();
	}

	int _lastSpawn = 0;
	Transform GetNextSpawn()
	{
		var spawnPoint = _spawnPoints[_lastSpawn];
		_lastSpawn = (_lastSpawn + 1) % _spawnPoints.Length;
		return spawnPoint;
	}

	public void SetActiveCharacter(GameObject character)
	{
		var expeditionCharacter = _characters.FirstOrDefault(c => c.GameObject == character);
		if (expeditionCharacter == null)
		{
			Debug.LogError($"Trying to set active character to {character.name} but it doesn't exist in the list of characters.");
			return;
		}
		_activeCharacter.Val = expeditionCharacter;
	}
}
