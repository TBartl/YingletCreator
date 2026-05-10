using Reactivity;
using System;
using System.Linq;
using UnityEngine;

public class SelectedOnMenu : ReactiveBehaviour, ISelectable, IInitializable
{

	[SerializeField] MenuType[] _menus;
	private IMenuManager _menuManager;
	private Computed<bool> _selected;

	public IReadOnlyObservable<bool> Selected => _selected;

	public void Initialize()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_selected = CreateComputed(ComputeSelected);
	}

	private bool ComputeSelected()
	{
		return _menus.Contains(_menuManager.OpenMenu.Val);
	}
}
