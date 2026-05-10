using Reactivity;
using System.Collections.Generic;
using System.Linq;

public enum RoundTransitionState
{
	None,
	TransitioningIn,
	IncrementRound
}

public interface IExpeditionRoundManager
{
	IReadOnlyObservable<int> CurrentRound { get; }
	IReadOnlyObservable<RoundTransitionState> TransitionState { get; }
	IEnumerable<ICharacterRoundState> CharacterRoundStates { get; }

	void SetTransitionState(RoundTransitionState state);
}

public class ExpeditionRoundManager : ReactiveBehaviour, IExpeditionRoundManager, IInitializable
{
	Observable<int> _currentRound = new Observable<int>(0);
	Observable<RoundTransitionState> _transitionState = new Observable<RoundTransitionState>(RoundTransitionState.None);
	private IExpeditionCharacterManager _expeditionCharacters;

	public IReadOnlyObservable<int> CurrentRound => _currentRound;

	private Computed<IEnumerable<ICharacterRoundState>> _characterRoundStates;
	public IEnumerable<ICharacterRoundState> CharacterRoundStates => _characterRoundStates.Val;
	public IReadOnlyObservable<RoundTransitionState> TransitionState => _transitionState;


	public void Initialize()
	{
		_expeditionCharacters = this.GetExpeditionComponent<IExpeditionCharacterManager>();
		_characterRoundStates = CreateComputed(ComputeCharacterRoundStates);
	}

	private IEnumerable<ICharacterRoundState> ComputeCharacterRoundStates()
	{
		return _expeditionCharacters.Characters
			.Select(c => c.Root.GetComponentInChildrenSafe<ICharacterRoundState>())
			.ToArray();
	}

	public void SetTransitionState(RoundTransitionState state)
	{
		_transitionState.Val = state;
		if (state == RoundTransitionState.IncrementRound)
		{
			_currentRound.Val++;
		}
	}
}
