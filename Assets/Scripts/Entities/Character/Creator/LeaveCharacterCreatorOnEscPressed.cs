using Character.Creator;
using System.IO;
using UnityEngine;

public class LeaveCharacterCreatorOnEscPressed : MonoBehaviour, IEscapeInputConsumer
{
	private IEscapeInputManager _escapeInputManager;
	private ICustomizationSelection _selection;
	private ICharacterCreatorTracker _characterCreatorTracker;
	private IMenuManager _menuManager;
	private IConfirmationManager _confirmationManager;
	private ISettingsManager _settingsManager;

	private void Awake()
	{
		_escapeInputManager = Singletons.GetSingleton<IEscapeInputManager>();
		_selection = Singletons.GetSingleton<ICustomizationSelection>();
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();

		_escapeInputManager.Register(this);
	}

	private void OnDestroy()
	{
		_escapeInputManager.Unregister(this);
	}

	public EscapeInputPriority EscapeInputPriority => EscapeInputPriority.CloseMenu;

	public bool OnEscape()
	{
		if (!_characterCreatorTracker.IsInCharacterCreator.Val) return false;

		if (_selection.SelectionIsDirty)
		{
			_confirmationManager.OpenConfirmation(new(
				"You have unsaved changes\n\nAre you sure you want to exit the character creator?",
				"Exit Anyway",
				"change-yinglet-selection",
				ExitCharacterCreator));
		}
		else
		{
			ExitCharacterCreator();
		}

		void ExitCharacterCreator()
		{
			_menuManager.PopMenu();

			// This probably shouldn't live here but w/e
			_settingsManager.Settings.LastSelectedCharacterPath = Path.GetFileNameWithoutExtension(_selection.Selected.Val.Path);
			_settingsManager.SaveChangesToDisk();
		}

		return true;
	}
}
