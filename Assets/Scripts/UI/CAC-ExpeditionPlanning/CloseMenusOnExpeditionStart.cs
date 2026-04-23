using System;
using System.Linq;
using UnityEngine;

public class CloseMenusOnExpeditionStart : MonoBehaviour
{
	[SerializeField] MenuType[] _menusToClose;

	private IMenuManager _menuManager;
	private IExpeditionManager _expeditionManager;

	void Start()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();

		_expeditionManager.State.OnChanged += OnExpeditionStateChanged;
	}

	private void OnExpeditionStateChanged(ExpeditionState from, ExpeditionState to)
	{
		if (to == ExpeditionState.Running)
		{
			while (_menusToClose.Contains(_menuManager.OpenMenu.Val))
			{
				_menuManager.PopMenu();
			}
		}
	}
}
