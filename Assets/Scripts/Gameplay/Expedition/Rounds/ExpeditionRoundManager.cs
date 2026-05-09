using Reactivity;
using System.Collections.Generic;
using System.Linq;

public interface IExpeditionRoundManager
{
	IReadOnlyObservable<int> CurrentRound { get; }
	IEnumerable<ICharacterRoundState> CharacterRoundStates { get; }
}

public class ExpeditionRoundManager : ReactiveBehaviour, IExpeditionRoundManager
{
	Observable<int> _currentRound = new Observable<int>(1);
	private IExpeditionCharacterManager _expeditionCharacters;
	private Computed<IEnumerable<ICharacterRoundState>> _characterRoundStates;
	private Computed<bool> _allCharactersAsleep;

	public IReadOnlyObservable<int> CurrentRound => _currentRound;
	public IEnumerable<ICharacterRoundState> CharacterRoundStates => _characterRoundStates.Val;

	void Start()
	{
		_expeditionCharacters = this.GetExpeditionComponent<IExpeditionCharacterManager>();
		_characterRoundStates = CreateComputed(ComputeCharacterRoundStates);
		_allCharactersAsleep = CreateComputed(ComputeAllCharactersAsleep);

		_allCharactersAsleep.OnChanged += OnAllCharactersAsleepChanged;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_allCharactersAsleep != null)
		{
			_allCharactersAsleep.OnChanged -= OnAllCharactersAsleepChanged;
		}
	}

	private IEnumerable<ICharacterRoundState> ComputeCharacterRoundStates()
	{
		return _expeditionCharacters.Characters
			.Select(c => c.Root.GetComponentInChildrenSafe<ICharacterRoundState>())
			.ToArray();
	}

	bool ComputeAllCharactersAsleep()
	{
		return _characterRoundStates.Val.All(c => c.IsAsleep.Val);
	}

	private void OnAllCharactersAsleepChanged(bool from, bool to)
	{
		if (to != true) return;
		_currentRound.Val += 1;
	}
}
