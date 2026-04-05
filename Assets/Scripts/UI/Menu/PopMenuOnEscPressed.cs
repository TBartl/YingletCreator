using UnityEngine;

public class PopMenuOnEscPressed : MonoBehaviour
{
	[SerializeField] MenuType _escapeMenu;
	private IMenuManager _menuManager;

	void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape)) return;
		if (_menuManager.OpenMenu.Val.PopOnEscape)
		{
			_menuManager.PopMenu();
		}
		else
		{
			_menuManager.PushMenu(_escapeMenu);
		}
	}
}
