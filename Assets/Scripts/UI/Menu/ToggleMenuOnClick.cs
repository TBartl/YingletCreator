using UnityEngine;
using UnityEngine.UI;

public class ToggleMenuOnClick : MonoBehaviour
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
		// Close it if it's already open
		if (_menuManager.OpenMenu.Val == _menuType)
		{
			_menuManager.OpenMenu.Val = null;
		}
		else
		{
			_menuManager.OpenMenu.Val = _menuType;
		}
	}
}
