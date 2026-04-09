using UnityEngine;

namespace Character.Creator
{
	internal class CharacterCreatorUndoSfx : MonoBehaviour
	{
		[SerializeField] private SoundEffect _undoApplied;
		[SerializeField] private SoundEffect _nothingToUndo;
		[SerializeField] private SoundEffect _redoApplied;
		[SerializeField] private SoundEffect _nothingToRedo;

		private IAudioPlayer _audioPlayer;
		private ICharacterCreatorUndoManager _undoManager;

		private void Awake()
		{
			_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();

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
			if (_undoApplied != null)
				_audioPlayer.Play(_undoApplied);
		}

		private void UndoManager_NothingToUndo()
		{
			if (_nothingToUndo != null)
				_audioPlayer.Play(_nothingToUndo);
		}

		private void UndoManager_RedoApplied(string obj)
		{
			if (_redoApplied != null)
				_audioPlayer.Play(_redoApplied);
		}

		private void UndoManager_NothingToRedo()
		{
			if (_nothingToRedo != null)
				_audioPlayer.Play(_nothingToRedo);
		}
	}
}
