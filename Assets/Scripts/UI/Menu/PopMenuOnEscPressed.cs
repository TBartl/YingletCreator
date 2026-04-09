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
		var interaction = _menuManager.OpenMenu.Val.EscapeInteraction;

		if (interaction == MenuEscInteraction.PopOnEscape)
		{
			_menuManager.PopMenu();
		}
		else if (interaction == MenuEscInteraction.OpenEscMenuOnEscape)
		{
			_menuManager.PushMenu(_escapeMenu);
		}
	}
}
