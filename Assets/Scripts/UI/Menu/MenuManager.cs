using Reactivity;
using UnityEngine;

/// <summary>
/// Handles which "menu" is being shown
/// Menus are now treated as a stack that things can be pushed / popped on
/// This allows for nesting of menus, such as opening a settings menu on top of the pause menu, and then closing it to return to the pause menu
/// But only one menu will ever be visible at a time
/// </summary>
public interface IMenuManager
{
	IReadOnlyObservable<MenuType> OpenMenu { get; }

	void PushMenu(MenuType menuType);
	void PopMenu();

	void InsertMenuAfter(MenuType menuToInsert, MenuType menuToInsertAfter);
	void RemoveMenu(MenuType menuType);
}

public class MenuManager : ReactiveBehaviour, IMenuManager
{
	[SerializeField] MenuType[] _defaultMenus;

	// Optimization opportunity: Could create ObservableStack
	ObservableList<MenuType> _menuStack = new();
	Computed<MenuType> _topMostMenu;


	public IReadOnlyObservable<MenuType> OpenMenu => _topMostMenu;

	private void Awake()
	{
		foreach (var menu in _defaultMenus)
		{
			_menuStack.Add(menu);
		}
		_topMostMenu = CreateComputed(ComputeTopMostMenu);
	}

	private MenuType ComputeTopMostMenu()
	{
		return _menuStack[_menuStack.Count - 1];
	}

	public void PopMenu()
	{
		_menuStack.RemoveAt(_menuStack.Count - 1);
		if (_menuStack.Count == 0)
		{
			Debug.LogError("Menu stack is empty! This should never happen");
		}
	}

	public void PushMenu(MenuType menuType)
	{
		_menuStack.Add(menuType);
	}

	public void InsertMenuAfter(MenuType menuToInsert, MenuType menuToInsertAfter)
	{
		int insertAfterIndex = _menuStack.IndexOf(menuToInsertAfter);
		if (insertAfterIndex == -1)
		{
			Debug.LogError($"Menu {menuToInsertAfter.name} not found in menu stack");
			return;
		}

		_menuStack.Insert(insertAfterIndex + 1, menuToInsert);
	}

	public void RemoveMenu(MenuType menuType)
	{
		int removeIndex = _menuStack.IndexOf(menuType);
		if (removeIndex == -1)
		{
			Debug.LogError($"Menu {menuType.name} not found in menu stack");
			return;
		}

		if (_menuStack.Count == 1)
		{
			Debug.LogError("Cannot remove the last menu from the stack");
			return;
		}

		_menuStack.RemoveAt(removeIndex);
	}
}


public static class MenuManagerExtensions
{
	public static void SwapTopToMenu(this IMenuManager menuManager, MenuType menuType)
	{
		using var suspend = new ReactivityNotificationSuspender();
		menuManager.PopMenu();
		menuManager.PushMenu(menuType);
	}
}