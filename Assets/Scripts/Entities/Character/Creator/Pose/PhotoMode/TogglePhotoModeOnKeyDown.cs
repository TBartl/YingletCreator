using UnityEngine;


namespace Character.Creator.UI
{
	public class TogglePhotoModeOnKeyDown : MonoBehaviour
	{
		private IMenuManager _menuManager;
		private IInputRestrictor _inputRestrictor;
		private IPhotoModeChecker _photoModeState;
		private IInPoseModeChecker _inPoseMode;

		private void Awake()
		{
			_menuManager = Singletons.GetSingleton<IMenuManager>();
			_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
			_photoModeState = this.GetComponent<IPhotoModeChecker>();
			_inPoseMode = this.GetComponentInChildren<IInPoseModeChecker>();
		}

		private void Update()
		{
			if (!_inputRestrictor.InputAllowed) return; // Input not allowed

			if (!Input.GetKeyDown(KeyCode.LeftControl)) return;// Key not pressed

			bool isPoseMode = _inPoseMode.InPoseMode.Val;
			if (!isPoseMode) return; // Not on pose mode

			var photoModeMenu = _photoModeState.PhotoModeMenu;
			if (_menuManager.OpenMenu.Val == photoModeMenu)
			{
				_menuManager.PopMenu();
			}
			else
			{
				_menuManager.PushMenu(photoModeMenu);
			}
		}
	}
}
