using UnityEngine;

public class LeaveCharacterCreatorOnEscPressed : MonoBehaviour
{
	[SerializeField] MenuType _characterCreatorMenu;

	private IMenuManager _menuManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	void Update()
	{
		if (_menuManager.OpenMenu.Val != _characterCreatorMenu) return;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// TODO: Intercept if anything is changed and push menu
			// Also check if that's open
			_menuManager.PopMenu();
		}
	}
}
