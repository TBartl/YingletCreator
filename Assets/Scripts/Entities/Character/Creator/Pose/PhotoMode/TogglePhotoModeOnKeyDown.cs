using UnityEngine;


namespace Character.Creator.UI
{
	public class TogglePhotoModeOnKeyDown : MonoBehaviour
	{
		private IInputRestrictor _inputRestrictor;
		private IPhotoModeState _photoModeState;
		private IInPoseModeChecker _inPoseMode;

		private void Awake()
		{
			_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
			_photoModeState = this.GetComponent<IPhotoModeState>();
			_inPoseMode = this.GetComponentInChildren<IInPoseModeChecker>();
		}

		private void Update()
		{
			if (!_inputRestrictor.InputAllowed) return; // Input not allowed

			// Only allow switching in pose mode, or if we somehow got to this state outside of it
			bool isPoseMode = _inPoseMode.InPoseMode.Val;
			bool inPhotoMode = _photoModeState.IsInPhotoMode.Val;
			if (isPoseMode || inPhotoMode)
			{
				if (Input.GetKeyDown(KeyCode.LeftControl))
				{
					_photoModeState.Toggle();
				}
			}
		}
	}
}