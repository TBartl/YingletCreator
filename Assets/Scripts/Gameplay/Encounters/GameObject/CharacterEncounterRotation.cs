using Reactivity;
using System;

public class CharacterEncounterRotation : ReactiveBehaviour
{
	private IRotateToVelocity _rotateToVelocity;
	private ICharacterEncounterReference _encounter;
	private Computed<EncounterCharacterPosition> _encounterPosition;
	private IDisposable _suspendAutoRotation;

	private void Start()
	{
		_rotateToVelocity = this.GetComponentSafe<IRotateToVelocity>();
		_encounter = this.GetCharacterRootComponent<ICharacterEncounterReference>();
		_encounterPosition = this.CreateComputed(() => _encounter.Encounter.Val?.EncounterSource?.GetComponentInChildren<EncounterCharacterPosition>());
		_encounterPosition.OnChanged += EncounterPositionChanged;
	}

	private void EncounterPositionChanged(EncounterCharacterPosition from, EncounterCharacterPosition to)
	{
		if (to != null)
		{
			this.transform.rotation = to.transform.rotation;
			if (_suspendAutoRotation == null) _suspendAutoRotation = _rotateToVelocity.SuspendAutoRotation();
		}
		else
		{
			_suspendAutoRotation?.Dispose();
			_suspendAutoRotation = null;
		}
	}
}
