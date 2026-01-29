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
		if (_confirmationManager.Current.Val != null)
		{
			// Want to restrict
			if (_heldRestriction != null) return; // but we're already restricting
			_heldRestriction = _inputRestrictor.RestrictInput();
		}
		else
		{
			// Input allowed
			if (_heldRestriction == null) return; // and we're already allowing
			_heldRestriction.Dispose();
			_heldRestriction = null;
		}
	}

}
