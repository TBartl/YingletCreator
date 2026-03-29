using UnityEngine;
using UnityEngine.UI;

public class SwapToMenuOnClick : MonoBehaviour
{
	[SerializeField] MenuType _menuType;

	private Button _button;
	private IMenuManager _menuManager;

	void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();

		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		var openMenu = _menuManager.OpenMenu.Val;
		if (openMenu == _menuType)
		{
			// Already on this, so pop back
			_menuManager.PopMenu();
		}
		else if (openMenu.SettingsSwapMenu && _menuType.SettingsSwapMenu)
		{
			// Both are settings, so swap instead of pushing
			_menuManager.SwapTopToMenu(_menuType);
		}
		else
		{
			_menuManager.PushMenu(_menuType);
		}
	}
}
