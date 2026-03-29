using UnityEngine;

public class PopMenuOnEscPressed : MonoBehaviour
{
	private IMenuManager _menuManager;

	void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape)) return;
		if (!_menuManager.OpenMenu.Val.PopOnEscape) return;
		_menuManager.PopMenu();
	}
}
