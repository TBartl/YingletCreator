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
		this._encounter.Val = encounter;
	}
}
