using UnityEngine;
using UnityEngine.UI;

public class BackdropPopMenuOnClick : MonoBehaviour
{
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
		if (openMenu.PopOnBackdropClicked)
		{
			_menuManager.PopMenu();
		}
	}
}
