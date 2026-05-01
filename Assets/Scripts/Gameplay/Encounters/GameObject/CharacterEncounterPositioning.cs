using Reactivity;
using UnityEngine;

public class CharacterEncounterPositioning : ReactiveBehaviour
{
	private Rigidbody _rb;
	private ICharacterEncounterReference _encounter;
	private Computed<EncounterCharacterPosition> _encounterPosition;

	private void Start()
	{
		_rb = this.GetCharacterRootComponent<Rigidbody>();
		_encounter = this.GetCharacterRootComponent<ICharacterEncounterReference>();
		_encounterPosition = this.CreateComputed(() => _encounter.Encounter.Val?.EncounterSource?.GetComponentInChildren<EncounterCharacterPosition>());
		_encounterPosition.OnChanged += EncounterPositionChanged;
	}

	private void EncounterPositionChanged(EncounterCharacterPosition from, EncounterCharacterPosition to)
	{
		if (to != null)
		{
			_rb.isKinematic = true;
			_rb.MovePosition(to.transform.position);
		}
		else
		{
			_rb.isKinematic = false;
		}
	}
}
