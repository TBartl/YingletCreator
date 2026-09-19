using System;
using UnityEngine;

public class DeleteNodeUIIfNotCurrent : MonoBehaviour
{
	private IEncounterNodeReferenceUI _reference;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_reference = this.GetComponentSafe<IEncounterNodeReferenceUI>();

		_reference.EncounterInstance.CurrentNode.OnChanged += CurrentNode_OnChanged;

		DestroyIfNotLatest();
	}

	private void OnDestroy()
	{
		if (_reference == null) return;
		_reference.EncounterInstance.CurrentNode.OnChanged -= CurrentNode_OnChanged;
	}

	private void CurrentNode_OnChanged(EncounterNodeVisitRecord from, EncounterNodeVisitRecord to)
	{
		throw new NotImplementedException();
	}

	void DestroyIfNotLatest()
	{
		int indexInHistory = _reference.IndexInHistory;
		if (indexInHistory < _reference.EncounterInstance.NodeHistory.Count - 1)
		{
			Destroy(gameObject);
		}
	}
}
