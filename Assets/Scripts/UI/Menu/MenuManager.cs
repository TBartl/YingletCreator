using Reactivity;
using UnityEngine;

/// <summary>
/// Handles which "menu" is being shown
/// </summary>
public interface IMenuManager
{
	Observable<MenuType> OpenMenu { get; }
}

public class MenuManager : MonoBehaviour, IMenuManager
{
	[SerializeField] MenuType _defaultMenu;

	Observable<MenuType> _openMenu;
	public Observable<MenuType> OpenMenu => _openMenu;

	private void Awake()
	{
		_openMenu = new Observable<MenuType>(_defaultMenu);
	}
}
