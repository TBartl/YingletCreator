using Reactivity;
using System;


public class RestrictInputOnSelectMenus : ReactiveBehaviour
{
	private IInputRestrictor _inputRestrictor;
	private IMenuManager _menuManager;

	private IDisposable _heldRestriction;

	private void Start()
	{
		_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
		_menuManager = Singletons.GetSingleton<IMenuManager>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		if (_menuManager.OpenMenu.Val.RestrictGameInput)
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
