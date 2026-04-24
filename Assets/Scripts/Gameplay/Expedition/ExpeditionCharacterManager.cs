using Character.Creator;
using Networking;
using Reactivity;
using System.Collections.Generic;
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
	IEnumerable<ExpeditionCharacter> Characters { get; }
}
public class ExpeditionCharacterManager : MonoBehaviour, IExpeditionCharacterManager
{
	[SerializeField] Transform[] _spawnPoints;

	ObservableList<ExpeditionCharacter> _characters = new ObservableList<ExpeditionCharacter>();
	private IExpeditionPlanningManager _expeditionPlanningManager;
	private ICharacterSpawner _characterSpawner;
	private ExpeditionRoot _root;
	private GameObject _parentObject;

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
		foreach (var partyMember in _expeditionPlanningManager.CurrentParty)
		{
			var gameObject = _characterSpawner.SpawnCharacter((gameObject) =>
			{
				gameObject.transform.SetParent(_parentObject.transform);
				gameObject.transform.position = GetNextSpawn().position;

				var identity = gameObject.GetComponentSafe<IWriteableCharacterIdentity>();
				identity.SetOwner(partyMember.ClientId);

				var dataRepo = gameObject.GetComponentInChildren<IForceableCustomizationDataRepository>();
				dataRepo.ForceCustomizationData(partyMember.CustomizationData);
			});

			_characters.Add(new ExpeditionCharacter(gameObject));
		}
	}

	int _lastSpawn = 0;
	Transform GetNextSpawn()
	{
		var spawnPoint = _spawnPoints[_lastSpawn];
		_lastSpawn = (_lastSpawn + 1) % _spawnPoints.Length;
		return spawnPoint;
	}
}
