using Encounters.Runtime;
using Reactivity;
using UnityEngine;

public class CreateUIPrefabsForEncounter : ReactiveBehaviour
{
	[SerializeField] GameObject _narrationPrefab;
	[SerializeField] GameObject _promptContinuePrefab;
	[SerializeField] GameObject _promptChoicesPrefab;
	[SerializeField] GameObject _rollPrefab;
	[SerializeField] GameObject _resourceChangedPrefab;
	[SerializeField] GameObject _statusAddedPrefab;

	int _nodeResultDataIndex;

	IActiveEncounterProvider _activeEncounterProvider;
	private IEncounterLogPositioner _positioner;

	void Awake()
	{
		_activeEncounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();
		_positioner = this.GetComponentSafe<IEncounterLogPositioner>();

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
			_nodeResultDataIndex = 0;
			_positioner.ResetPosition();
			to.CurrentNode.OnChanged += OnEncounterNodeChanged;

			// Catch up
			for (int i = 0; i < to.NodeHistory.Count; i++)
			{
				var node = to.NodeHistory[i];
				CreateObjectForNode(node, i);
			}
		}
	}

	private void OnEncounterNodeChanged(EncounterNodeVisitRecord from, EncounterNodeVisitRecord to)
	{
		if (to == null) return;
		int indexInHistory = _activeEncounterProvider.ActiveEncounter.Val.NodeHistory.Count - 1;
		CreateObjectForNode(to, indexInHistory);
	}



	void CreateObjectForNode(EncounterNodeVisitRecord record, int indexInHistory)
	{
		var encounter = _activeEncounterProvider.ActiveEncounter.Val;
		var node = record.Node;

		var prefab = GetPrefabForNode(node);
		if (prefab == null) return; // Not every node has a UI representation
		var obj = Instantiate(prefab, transform);
		obj.GetComponentSafe<IEncounterNodeReferenceUI>().SetReference(encounter, record, indexInHistory);
		bool closerToTheBottom = IsCloserToBottom(node);
		_positioner.ObjectAdded(closerToTheBottom);


		// TTODO
		//else if (node is RollBlockNode rollBlockNode)
		//{
		//	// We create the UI when the block has been selected since that's when all the data is available
		//	// Figure out the note that originated it
		//	var rollNode = (RollNode)(encounter.NodeHistory[indexInHistory - 1]);
		//	GameObject rollObject = Instantiate(_rollPrefab, transform);
		//	SetReferenceUI(rollObject);
		//	rollObject.GetComponentInChildrenSafe<IRollUI>().SetNode(encounter, rollNode, rollBlockNode, GetNextData(encounter));
		//	_positioner.ObjectAdded(false);
		//}
	}

	GameObject GetPrefabForNode(IEncounterNode node)
	{
		if (node is NarrationNode) return _narrationPrefab;
		if (node is PromptContinueNode) return _promptContinuePrefab;
		if (node is PromptChoiceNode) return _promptChoicesPrefab;
		if (node is RollBlockNode) return _rollPrefab;
		if (node is ChangeCharacterResourceNode) return _resourceChangedPrefab;
		if (node is AddStatusToCharacterNode) return _statusAddedPrefab;
		return null;
	}

	public bool IsCloserToBottom(IEncounterNode node)
	{
		if (node is PromptChoiceNode)
		{
			return true;
		}
		return false;
	}

	object GetNextData(IEncounterInstance encounter)
	{
		var data = encounter.NodeResultData[_nodeResultDataIndex];
		_nodeResultDataIndex++;
		return data;
	}

	void DestroyAllChildren()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
}
