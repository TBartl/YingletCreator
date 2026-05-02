using Encounters.Runtime;
using Reactivity;
using UnityEngine;

public class CreateUIPrefabsForEncounter : ReactiveBehaviour
{
	[SerializeField] GameObject _narrationPrefab;
	IActiveEncounterProvider _activeEncounterProvider;

	void Awake()
	{
		_activeEncounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();

		DestroyAllChildren();
		_activeEncounterProvider.ActiveEncounter.OnChanged += OnActiveEncounterChanged;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_activeEncounterProvider != null)
		{
			_activeEncounterProvider.ActiveEncounter.OnChanged -= OnActiveEncounterChanged;
		}
	}

	private void OnActiveEncounterChanged(IEncounterInstance from, IEncounterInstance to)
	{
		DestroyAllChildren();

		if (from != null)
		{
			from.CurrentNode.OnChanged -= OnEncounterNodeChanged;
		}

		if (to != null)
		{
			to.CurrentNode.OnChanged += OnEncounterNodeChanged;
		}
	}

	private void OnEncounterNodeChanged(IEncounterNode from, IEncounterNode to)
	{
		if (to == null) return;
		CreateObjectForNode(to);

	}

	void CreateObjectForNode(IEncounterNode node)
	{
		if (node is NarrationNode narrationNode)
		{
			GameObject narrationObject = Instantiate(_narrationPrefab, transform);
			narrationObject.GetComponentInChildrenSafe<TMPro.TMP_Text>().SetText(narrationNode.Text);
		}
	}

	void DestroyAllChildren()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
}
