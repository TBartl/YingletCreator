using Reactivity;
using UnityEngine;

public interface ICharacterRoundState
{
	IReadOnlyObservable<bool> IsAsleep { get; }

	void GoToSleep();
}

public class CharacterRoundState : MonoBehaviour, ICharacterRoundState
{
	private Observable<bool> _isAsleep = new Observable<bool>(false);
	private IExpeditionRoundManager _roundManager;
	private ICharacterEncounterReference _encounterReference;

	public IReadOnlyObservable<bool> IsAsleep => _isAsleep;

	private void Start()
	{
		_roundManager = this.GetExpeditionComponent<IExpeditionRoundManager>();
		_encounterReference = this.GetExpeditionComponent<ICharacterEncounterReference>();
		_roundManager.CurrentRound.OnChanged += OnRoundChanged;
	}

	private void OnRoundChanged(int from, int to)
	{
		// Wake up when the round changes
		_isAsleep.Val = false;
	}

	public void GoToSleep()
	{
		if (_encounterReference.Encounter.Val != null) return; // Don't allow sleeping if we're in an encounter
		_isAsleep.Val = true;
	}
}
