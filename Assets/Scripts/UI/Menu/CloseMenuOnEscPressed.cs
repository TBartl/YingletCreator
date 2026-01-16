using UnityEngine;

public class CloseMenuOnEscPressed : MonoBehaviour
{
	private IMenuManager _menuManager;

	void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_menuManager.OpenMenu.Val = null;
		}
	}
}
