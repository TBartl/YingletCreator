using UnityEngine;

public class LeaveCharacterCreatorOnEscPressed : MonoBehaviour
{
	private ICharacterCreatorTracker _characterCreatorTracker;
	private IMenuManager _menuManager;

	private void Awake()
	{
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	void Update()
	{
		if (!_characterCreatorTracker.IsInCharacterCreator.Val) return;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// TODO: Intercept if anything is changed and push menu
			// Also check if that's open
			_menuManager.PopMenu();
		}
	}
}
