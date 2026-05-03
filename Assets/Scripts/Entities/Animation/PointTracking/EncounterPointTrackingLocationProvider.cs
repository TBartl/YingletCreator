using Reactivity;
using UnityEngine;

public class EncounterPointTrackingLocationProvider : ReactiveBehaviour, IInitializable, IPointTrackingLocationProvider
{
	private CharacterEncounterReference _encounter;
	private Computed<EncounterCharacterLookAtPosition> _point;
	private Computed<bool> _active;

	public IReadOnlyObservable<bool> Active => _active;

	public Vector3 Position => _point.Val?.transform?.position ?? Vector3.zero;

	public bool MoveUpperBody => false;

	public void Initialize()
	{
		_encounter = this.GetCharacterRootComponent<CharacterEncounterReference>();

		_point = CreateComputed(ComputeLookAtPos);
		_active = CreateComputed(() => _point.Val != null);
	}

	private EncounterCharacterLookAtPosition ComputeLookAtPos()
	{
		var encounter = _encounter.Encounter.Val;
		if (encounter == null) return null;
		// Might be null
		return encounter.EncounterSource.GetComponentInChildren<EncounterCharacterLookAtPosition>();
	}
}
