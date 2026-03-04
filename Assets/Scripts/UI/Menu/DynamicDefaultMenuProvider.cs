using Reactivity;
using UnityEngine;

/// <summary>
/// CAC specific implementation of IDefaultMenuProvider that starts on the main menu until relevant
/// </summary>
public class DynamicDefaultMenuProvider : ReactiveBehaviour, IDefaultMenuProvider
{
	[SerializeField] MenuType _mainMenu;
	[SerializeField] MenuType _inGame;
	// Probably one for the character creator eventually?

	public MenuType DefaultMenu => _defaultMenu.Val;

	private IMainMenuState _mainMenuState;
	Computed<MenuType> _defaultMenu;
	private void Awake()
	{

		_mainMenuState = Singletons.GetSingleton<IMainMenuState>();
		_defaultMenu = CreateComputed(ComputeDefaultMenu);
	}

	private MenuType ComputeDefaultMenu()
	{
		if (_mainMenuState.OnMainMenu)
		{
			return _mainMenu;
		}
		return _inGame;
	}
}
