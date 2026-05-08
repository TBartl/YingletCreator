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

	private void OnEncounterNodeChanged(IEncounterNode from, IEncounterNode to)
	{
		if (to == null) return;
		int indexInHistory = _activeEncounterProvider.ActiveEncounter.Val.NodeHistory.Count - 1;
		CreateObjectForNode(to, indexInHistory);
	}



	void CreateObjectForNode(IEncounterNode node, int indexInHistory)
	{
		var encounter = _activeEncounterProvider.ActiveEncounter.Val;

		if (node is NarrationNode narrationNode)
		{
			GameObject narrationObject = Instantiate(_narrationPrefab, transform);
			SetReferenceUI(narrationObject);
			narrationObject.GetComponentInChildrenSafe<INarrationTextBox>().SetNode(encounter, narrationNode);
			_positioner.ObjectAdded(false);
		}
		else if (node is PromptContinueNode)
		{
			var go = Instantiate(_promptContinuePrefab, transform);
			SetReferenceUI(go);
			_positioner.ObjectAdded(false);
		}
		else if (node is PromptChoiceNode promptChoiceNode)
		{
			GameObject promptChoicesObject = Instantiate(_promptChoicesPrefab, transform);
			SetReferenceUI(promptChoicesObject);
			promptChoicesObject.GetComponentInChildrenSafe<IPromptChoicesUI>().SetNode(encounter, promptChoiceNode);
			_positioner.ObjectAdded(true);
		}
		else if (node is RollBlockNode rollBlockNode)
		{
			// We create the UI when the block has been selected since that's when all the data is available
			// Figure out the note that originated it
			var rollNode = (RollNode)(encounter.NodeHistory[indexInHistory - 1]);
			GameObject rollObject = Instantiate(_rollPrefab, transform);
			SetReferenceUI(rollObject);
			rollObject.GetComponentInChildrenSafe<IRollUI>().SetNode(encounter, rollNode, rollBlockNode, GetNextData(encounter));
			_positioner.ObjectAdded(false);
		}
		else if (node is ChangeCharacterResourceNode changeCharacterResourceNode)
		{
			GameObject resourceChangeObject = Instantiate(_resourceChangedPrefab, transform);
			SetReferenceUI(resourceChangeObject);
			resourceChangeObject.GetComponentInChildrenSafe<IResourceChangeBox>().SetNode(encounter, changeCharacterResourceNode);
			_positioner.ObjectAdded(false);
		}
		else if (node is AddStatusToCharacterNode addStatusToCharacterNode)
		{
			GameObject statusAddedObject = Instantiate(_statusAddedPrefab, transform);
			SetReferenceUI(statusAddedObject);
			statusAddedObject.GetComponentInChildrenSafe<IStatusAddedBox>().SetNode(encounter, addStatusToCharacterNode);
			_positioner.ObjectAdded(false);
		}

		void SetReferenceUI(GameObject obj)
		{
			obj.GetComponentSafe<IEncounterNodeReferenceUI>().SetReference(encounter, indexInHistory);
		}
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
