using UnityEngine;
using UnityEngine.EventSystems;

public class CloseMenuOnClick : MonoBehaviour, IPointerClickHandler
{
	private IMenuManager _menuManager;

	void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_menuManager.OpenMenu.Val = null;
	}
}
