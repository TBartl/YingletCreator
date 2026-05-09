using Reactivity;
using TMPro;

internal class ReflectSleepOrWakeUpText : ReactiveBehaviour
{
	private IGlobalRoundProvider _globalRoundProvider;
	private TMP_Text _text;

	Computed<bool> _isSleeping;

	private void Start()
	{
		_globalRoundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		_text = this.GetComponentSafe<TMP_Text>();

		_isSleeping = CreateComputed(ComputeIsSleeping);


		AddReflector(Reflect);
	}

	private bool ComputeIsSleeping()
	{
		return _globalRoundProvider.ActiveCharacterState?.IsAsleep?.Val ?? false;
	}

	private void Reflect()
	{
		_text.text = _isSleeping.Val ? "Wake Up" : "End Turn";
	}
}
