using Reactivity;
using UnityEngine;

public interface ICharacterEncounterReference
{
	IReadOnlyObservable<IEncounterInstance> Encounter { get; }

	void SetEncounter(IEncounterInstance encounter);
}

public class CharacterEncounterReference : MonoBehaviour, ICharacterEncounterReference
{
	Observable<IEncounterInstance> _encounter = new Observable<IEncounterInstance>();

	public IReadOnlyObservable<IEncounterInstance> Encounter => _encounter;

	public void SetEncounter(IEncounterInstance encounter)
	{
		if (this._encounter.Val != null)
		{
			Debug.LogError("Attempting to set encounter reference when it already has an encounter.");
			return;
		}
		this._encounter.Val = encounter;

		_encounter.Val.OnFinished += Encounter_OnFinished;
	}

	private void Encounter_OnFinished()
	{
		_encounter.Val.OnFinished -= Encounter_OnFinished;
		_encounter.Val = null;
	}
}
