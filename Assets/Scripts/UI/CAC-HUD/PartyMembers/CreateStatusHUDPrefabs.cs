using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateStatusHUDPrefabs : ReactiveBehaviour
{
	[SerializeField] GameObject _statusPrefab;

	EnumerableDictReflector<StatusId, GameObject> _dictReflector;
	private IActiveCharacterProvider _activeCharacterProvider;

	private void Start()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_dictReflector = new EnumerableDictReflector<StatusId, GameObject>(Added, Removed);
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
		AddReflector(Reflect);
	}

	private GameObject Added(StatusId id)
	{
		var go = GameObject.Instantiate(_statusPrefab, transform);
		go.GetComponent<IStatusHUD>().SetStatus(id);
		return go;
	}

	private void Removed(GameObject gameObject)
	{
		GameObject.Destroy(gameObject);
	}

	private void Reflect()
	{
		_dictReflector.Enumerate(GetStatuses());
	}
	IEnumerable<StatusId> GetStatuses()
	{
		var activeCharacter = _activeCharacterProvider.ActiveCharacter.Val;
		if (activeCharacter == null) return Enumerable.Empty<StatusId>();
		var characterStatuses = activeCharacter.GetComponentInChildrenSafe<ICharacterStatuses>();
		return characterStatuses.Statuses;
	}
}
