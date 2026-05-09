using Reactivity;
using System;


public class RestrictInputOnConfirmationManager : ReactiveBehaviour
{
	private IInputRestrictor _inputRestrictor;
	private IConfirmationManager _confirmationManager;

	private IDisposable _heldRestriction;

	private void Start()
	{
		_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		bool wantsToRestrict = _confirmationManager.Current.Val != null;
		_heldRestriction = _heldRestriction.Toggle(wantsToRestrict, _inputRestrictor.RestrictInput);
	}

}
