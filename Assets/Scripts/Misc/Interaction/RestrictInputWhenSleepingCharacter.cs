using Reactivity;
using System;


public class RestrictInputWhenSleepingCharacter : ReactiveBehaviour
{
	private IInputRestrictor _inputRestrictor;
	private IActiveCharacterProvider _activeCharacterProvider;
	private Computed<bool> _activeCharacterIsAsleep;
	private IDisposable _heldRestriction;

	private void Start()
	{
		_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();

		_activeCharacterIsAsleep = CreateComputed(ComputeActiveCharacterIsAsleep);

		AddReflector(Reflect);
	}

	private bool ComputeActiveCharacterIsAsleep()
	{
		var activeCharacter = _activeCharacterProvider.ActiveExpeditionCharacter.Val;
		if (activeCharacter == null) return false;

		bool isAsleep = activeCharacter.GetComponentInChildrenSafe<ICharacterRoundState>().IsAsleep.Val;
		return isAsleep;
	}

	private void Reflect()
	{
		bool wantsToRestrict = _activeCharacterIsAsleep.Val;
		_heldRestriction = _heldRestriction.Toggle(wantsToRestrict, _inputRestrictor.RestrictInput);
	}

}
