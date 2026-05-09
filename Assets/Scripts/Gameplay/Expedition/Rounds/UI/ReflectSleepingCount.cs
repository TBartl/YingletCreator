using Reactivity;
using System.Linq;
using TMPro;

internal class ReflectSleepingCount : ReactiveBehaviour
{
	private IGlobalRoundProvider _globalRoundProvider;
	private TMP_Text _text;
	Computed<int> _numSleeping;
	Computed<int> _total;

	private void Start()
	{
		_globalRoundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		_text = this.GetComponentSafe<TMP_Text>();
		_numSleeping = CreateComputed(ComputeNumSleeping);
		_total = CreateComputed(() => _globalRoundProvider.RoundManager?.CharacterRoundStates?.Count() ?? 0);

		AddReflector(Reflect);
	}

	private int ComputeNumSleeping()
	{
		return _globalRoundProvider.RoundManager?.CharacterRoundStates?.Count(c => c.IsAsleep.Val) ?? 0;
	}

	private void Reflect()
	{
		var text = $"({_numSleeping.Val}/{_total.Val})";
		_text.text = text;
	}
}
