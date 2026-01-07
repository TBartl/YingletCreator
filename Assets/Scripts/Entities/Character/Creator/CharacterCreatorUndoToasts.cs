using UnityEngine;

namespace Character.Creator
{
	internal class CharacterCreatorUndoToasts : MonoBehaviour
	{
		private IToastManager _toastDisplay;
		private ICharacterCreatorUndoManager _undoManager;

		private void Awake()
		{
			_toastDisplay = Singletons.GetSingleton<IToastManager>();
			_undoManager = this.GetComponentInParent<ICharacterCreatorUndoManager>();

			_undoManager.UndoApplied += UndoManager_UndoApplied;
			_undoManager.NothingToUndo += UndoManager_NothingToUndo;
			_undoManager.RedoApplied += UndoManager_RedoApplied;
			_undoManager.NothingToRedo += UndoManager_NothingToRedo;
		}

		private void OnDestroy()
		{
			_undoManager.UndoApplied -= UndoManager_UndoApplied;
			_undoManager.NothingToUndo -= UndoManager_NothingToUndo;
			_undoManager.RedoApplied -= UndoManager_RedoApplied;
			_undoManager.NothingToRedo -= UndoManager_NothingToRedo;
		}

		private void UndoManager_UndoApplied(string obj)
		{
			_toastDisplay.Show($"Undid action: {obj}");
		}

		private void UndoManager_NothingToUndo()
		{
			_toastDisplay.Show("Nothing to undo!");
		}

		private void UndoManager_RedoApplied(string obj)
		{
			_toastDisplay.Show($"Redid action: {obj}");
		}

		private void UndoManager_NothingToRedo()
		{
			_toastDisplay.Show("Nothing to redo!");
		}
	}
}
