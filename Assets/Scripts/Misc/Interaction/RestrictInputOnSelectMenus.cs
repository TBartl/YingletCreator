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
		bool wantsToRestrict = _menuManager.OpenMenu.Val.RestrictGameInput;
		_heldRestriction = _heldRestriction.Toggle(wantsToRestrict, _inputRestrictor.RestrictInput);
	}
}
