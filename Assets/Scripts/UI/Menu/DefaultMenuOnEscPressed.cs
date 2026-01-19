using UnityEngine;

public class DefaultMenuOnEscPressed : MonoBehaviour
{
	private IMenuManager _menuManager;
	private IDefaultMenuProvider _defaultMenuProvider;

	void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_defaultMenuProvider = this.GetComponent<IDefaultMenuProvider>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_menuManager.OpenMenu.Val = _defaultMenuProvider.DefaultMenu;
		}
	}
}
