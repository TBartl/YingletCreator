
using Character.Creator.UI;
using UnityEngine;

namespace Character.Creator
{

	public class CharacterCreatorUndoInput : MonoBehaviour
	{
		private IInputRestrictor _inputRestrictor;
		private ICharacterCreatorUndoManager _undoManager;
		private IInPoseModeChecker _inPoseMode;

		private void Awake()
		{
			_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_inPoseMode = this.GetCharacterCreatorComponent<IInPoseModeChecker>();
		}

		private void Update()
		{
			if (!_inputRestrictor.InputAllowed) return; // Input not allowed
			if (!Input.GetKey(KeyCode.LeftControl)) return; // Need to hold ctrl
			if (_inPoseMode.InPoseMode.Val) return; // Need to not be in pose mode

			if (Input.GetKeyDown(KeyCode.Z))
			{
				// Ctrl + Shift + Z = Redo as well
				if (Input.GetKey(KeyCode.LeftShift))
				{
					_undoManager.TryRedo();
					return;
				}
				_undoManager.TryUndo();
			}
			else if (Input.GetKeyDown(KeyCode.Y))
			{
				_undoManager.TryRedo();
			}
		}
	}
}