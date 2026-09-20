using Encounters.Runtime;
using Reactivity;
using System.Collections.Generic;
using UnityEngine;

public class SelectedOnRollUIState : ReactiveBehaviour, IInitializable, ISelectable
{
	[SerializeField] List<RollState> _selectedStates;

	private RollNodeVisitData _data;
	Computed<bool> _selected;

	public IReadOnlyObservable<bool> Selected => _selected;

	public void Initialize()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record?.VisitData as RollNodeVisitData;

		_selected = CreateComputed<bool>(ComputeSelected);
	}

	private bool ComputeSelected()
	{
		var state = _data.State;
		return _selectedStates.Contains(state);

	}
}
