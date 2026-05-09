using Character.Creator;
using Networking;
using Reactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public sealed class ExpeditionCharacter
{
	public ExpeditionCharacter(ICharacterRoot root)
	{
		Root = root;
		_identity = root.GetComponentInChildrenSafe<ICharacterIdentity>();
	}

	public ICharacterRoot Root;
	public ulong ClientId => _identity.OwnerClientId;
	public bool IsMine => _identity.IsMine;
	private ICharacterIdentity _identity;
}

public interface IExpeditionCharacterManager
{
	IReadOnlyObservable<ExpeditionCharacter> ActiveCharacter { get; }
	IEnumerable<ExpeditionCharacter> Characters { get; }

	void SetActiveCharacter(ICharacterRoot character);

	/// <summary>
	/// Attempts to select the next logical player
	/// Prioritizes players that are our own and aren't asleep
	/// </summary>
	void TryTabToNextCharacter();
}
public class ExpeditionCharacterManager : MonoBehaviour, IExpeditionCharacterManager
{
	[SerializeField] Transform[] _spawnPoints;

	Observable<ExpeditionCharacter> _activeCharacter = new Observable<ExpeditionCharacter>();
	ObservableList<ExpeditionCharacter> _characters = new ObservableList<ExpeditionCharacter>();
	private INetStateReader _netState;
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

			_characters.Add(new ExpeditionCharacter(gameObject.GetComponentSafe<ICharacterRoot>()));
		}
		_activeCharacter.Val = _characters.FirstOrDefault(c => c.IsMine);
	}

	int _lastSpawn = 0;
	Transform GetNextSpawn()
	{
		var spawnPoint = _spawnPoints[_lastSpawn];
		_lastSpawn = (_lastSpawn + 1) % _spawnPoints.Length;
		return spawnPoint;
	}

	public void SetActiveCharacter(ICharacterRoot character)
	{
		var expeditionCharacter = _characters.FirstOrDefault(c => c.Root == character);
		if (expeditionCharacter == null)
		{
			Debug.LogError($"Trying to set active character to {character.name} but it doesn't exist in the list of characters.");
			return;
		}
		_activeCharacter.Val = expeditionCharacter;
	}

	public void TryTabToNextCharacter()
	{
		var possibleCharacters = Characters.ToArray();
		var currentCharacter = ActiveCharacter.Val;

		var sleepingCharacters = Characters.Where(c => c.Root.GetComponentInChildrenSafe<ICharacterRoundState>().IsAsleep.Val).ToHashSet();

		// First, see if we can get one of our own characters that isn't asleep
		int currentCharacterIndex = Array.IndexOf(possibleCharacters, currentCharacter);
		for (int i = 1; i <= possibleCharacters.Length; i++)
		{
			var nextCharacter = possibleCharacters[(currentCharacterIndex + i) % possibleCharacters.Length];
			if (nextCharacter.IsMine && !sleepingCharacters.Contains(nextCharacter))
			{
				SetActiveCharacter(nextCharacter.Root);
				return;
			}
		}
		// If we can't, see if we can get any character that isn't asleep
		for (int i = 1; i <= possibleCharacters.Length; i++)
		{
			var nextCharacter = possibleCharacters[(currentCharacterIndex + i) % possibleCharacters.Length];
			if (!sleepingCharacters.Contains(nextCharacter))
			{
				SetActiveCharacter(nextCharacter.Root);
				return;
			}
		}

		// If we can't, don't change the active character
	}
}
