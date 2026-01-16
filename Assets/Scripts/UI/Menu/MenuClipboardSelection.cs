using Reactivity;
using UnityEngine;

public class MenuClipboardSelection : ReactiveBehaviour, ISelectable
{
	[SerializeField] MenuType _menuType;
	private IMenuManager _menuManager;
	Computed<bool> _selected;
	public IReadOnlyObservable<bool> Selected => _selected;

	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_selected = CreateComputed(ComputeSelected);
	}

	private bool ComputeSelected()
	{
		return _menuManager.OpenMenu.Val == _menuType;
	}
}
