using Reactivity;
using UnityEngine;


internal interface IPointTrackingForcer
{
	bool Forcing { get; }
}

// Currently just forces on screensaver
internal class PointTrackingForcer : ReactiveBehaviour, IPointTrackingForcer
{
	[SerializeReference] MenuType _screensaverMenuType;
	private IMenuManager _menuManager;
	Computed<bool> _forcing;

	public bool Forcing => _forcing.Val;

	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_forcing = CreateComputed(() =>
		{
			return _menuManager.OpenMenu.Val == _screensaverMenuType;
		});

	}
}
