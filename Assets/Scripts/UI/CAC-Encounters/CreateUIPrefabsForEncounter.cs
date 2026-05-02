using Encounters.Runtime;
using Reactivity;
using UnityEngine;

public class CreateUIPrefabsForEncounter : ReactiveBehaviour
{
	[SerializeField] GameObject _narrationPrefab;
	[SerializeField] GameObject _promptContinuePrefab;
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

		if (from != null)
		{
			from.CurrentNode.OnChanged -= OnEncounterNodeChanged;
		}

		if (to != null)
		{
			DestroyAllChildren(); // We don't want to always do this - when we're transitioning out we want to leave the UI on screen
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
			narrationObject.GetComponentInChildrenSafe<INarrationTextBox>().SetNode(_activeEncounterProvider.ActiveEncounter.Val, narrationNode);
		}
		else if (node is PromptContinueNode)
		{
			Instantiate(_promptContinuePrefab, transform);
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
