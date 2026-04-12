using Character.Creator;
using UnityEngine;

public class LeaveCharacterCreatorOnEscPressed : MonoBehaviour
{
	private ICustomizationSelection _selection;
	private ICharacterCreatorTracker _characterCreatorTracker;
	private IMenuManager _menuManager;
	private IConfirmationManager _confirmationManager;

	private void Awake()
	{
		_selection = Singletons.GetSingleton<ICustomizationSelection>();
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
	}

	void Update()
	{
		if (!_characterCreatorTracker.IsInCharacterCreator.Val) return;
		if (!Input.GetKeyDown(KeyCode.Escape)) return;

		if (_selection.SelectionIsDirty)
		{
			_confirmationManager.OpenConfirmation(new(
				"You have unsaved changes\n\nAre you sure you want to exit the character creator?",
				"Exit Anyway",
				"change-yinglet-selection",
				PopMenu));
		}
		else
		{
			PopMenu();
		}

		void PopMenu()
		{
			_menuManager.PopMenu();
		}
	}
}
