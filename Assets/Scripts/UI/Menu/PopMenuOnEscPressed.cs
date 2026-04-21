using UnityEngine;

public class PopMenuOnEscPressed : MonoBehaviour, IEscapeInputConsumer
{
	[SerializeField] MenuType _escapeMenu;
	private IEscapeInputManager _escapeInputManager;
	private IMenuManager _menuManager;


	void Awake()
	{
		_escapeInputManager = Singletons.GetSingleton<IEscapeInputManager>();
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_escapeInputManager.Register(this);
	}
	private void OnDestroy()
	{
		_escapeInputManager.Unregister(this);
	}

	public EscapeInputPriority EscapeInputPriority => EscapeInputPriority.CloseMenu;

	public bool OnEscape()
	{
		var interaction = _menuManager.OpenMenu.Val.EscapeInteraction;

		if (interaction == MenuEscInteraction.PopOnEscape)
		{
			_menuManager.PopMenu();
			return true;
		}
		else if (interaction == MenuEscInteraction.OpenEscMenuOnEscape)
		{
			_menuManager.PushMenu(_escapeMenu);
			return true;
		}
		return false;
	}
}
