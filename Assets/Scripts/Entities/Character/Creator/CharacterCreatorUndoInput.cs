using UnityEngine;

namespace Character.Creator
{

	public class CharacterCreatorUndoInput : MonoBehaviour
	{
		private ICharacterCreatorTracker _characterCreatorTracker;
		private ICharacterCreatorUndoManager _undoManager;

		private void Awake()
		{
			_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
		}

		private void Update()
		{
			if (!_characterCreatorTracker.IsInCharacterCreator.Val) return;
			if (!Input.GetKey(KeyCode.LeftControl)) return; // Need to hold ctrl

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