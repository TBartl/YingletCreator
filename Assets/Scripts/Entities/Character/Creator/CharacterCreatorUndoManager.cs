using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Creator
{
	public interface ICharacterCreatorUndoManager
	{
		/// <summary>
		/// Consumers of this will typically be the actual UI buttons and things
		/// This is because the DataRepository doesn't know the context of the action
		/// i.e.: State should only be recorded when a slider starts to be dragged
		/// </summary>
		void RecordState(string action);

		void TryUndo();
		void TryRedo();

		/// <summary>
		/// Event fired when an undo was applied with a description of the operation
		/// </summary>
		event Action<string> UndoApplied;

		/// <summary>
		/// Event fired when an undo was attempted but there was nothing to undo
		/// </summary>
		event Action NothingToUndo;

		event Action<string> RedoApplied;
		event Action NothingToRedo;
	}

	public class CharacterCreatorUndoManager : MonoBehaviour, ICharacterCreatorUndoManager
	{
		private ICharacterCreatorStateSnapshotter _stateSnapshotter;
		ICustomizationSelection _customizationSelection;

		List<CharacterCreatorStateSnapshot> _undoStack = new(); // Not actually a stack because we need to remove things from the front
		List<CharacterCreatorStateSnapshot> _redoStack = new(); // Redo stack to store undone states
		private ICharacterCreatorTracker _characterCreatorTracker;

		public event Action<string> UndoApplied;
		public event Action NothingToUndo;
		public event Action<string> RedoApplied;
		public event Action NothingToRedo;

		private void Awake()
		{
			_stateSnapshotter = Singletons.GetSingleton<ICharacterCreatorStateSnapshotter>();
			_customizationSelection = Singletons.GetSingleton<ICustomizationSelection>();
		}

		private void Start()
		{
			_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
			_characterCreatorTracker.IsInCharacterCreator.OnChanged += Tracker_OnIsInCharacterCreatorChanged;
		}

		private void OnDestroy()
		{
			_characterCreatorTracker.IsInCharacterCreator.OnChanged -= Tracker_OnIsInCharacterCreatorChanged;
		}

		private void Tracker_OnIsInCharacterCreatorChanged(bool from, bool to)
		{
			_undoStack.Clear();
			_redoStack.Clear();
		}

		public void RecordState(string action)
		{
			var undoState = _stateSnapshotter.GetStateSnapshot(action);
			_undoStack.Add(undoState);

			// Clear redo stack when new state is recorded
			_redoStack.Clear();
			ClampStacksToMax();

			// It probably shouldn't be the responsibility of the undo manager to also keep track of what's "dirty", but this is a really convenient chokepoint for all that logic
			_customizationSelection.SelectionIsDirty = true;
		}

		public void TryRedo()
		{
			// Check if we even have anything to redo
			if (!_redoStack.Any())
			{
				NothingToRedo?.Invoke();
				return;
			}

			var futureState = _redoStack.Last();
			_redoStack.RemoveAt(_redoStack.Count - 1);

			// Before we apply the future state, save the current state to the undo stack
			// There's probably some optimization that could be done here but whatever
			var currentState = _stateSnapshotter.GetStateSnapshot(futureState.Action);
			_undoStack.Add(currentState);

			_stateSnapshotter.RestoreStateSnapshot(futureState);
			RedoApplied?.Invoke(futureState.Action);
			ClampStacksToMax();
		}

		public void TryUndo()
		{
			// Check if we even have anything
			if (!_undoStack.Any())
			{
				NothingToUndo?.Invoke();
				return;
			}

			var previousState = _undoStack.Last();
			_undoStack.RemoveAt(_undoStack.Count - 1);

			// Before we apply the previous state, save the current state to the redo stack
			var currentState = _stateSnapshotter.GetStateSnapshot(previousState.Action);
			_redoStack.Add(currentState);

			_stateSnapshotter.RestoreStateSnapshot(previousState);
			UndoApplied?.Invoke(previousState.Action);
			ClampStacksToMax();
		}

		const int MAX_STACK_SIZE = 16;
		void ClampStacksToMax()
		{
			if (_undoStack.Count > MAX_STACK_SIZE)
			{
				_undoStack.RemoveRange(0, _undoStack.Count - MAX_STACK_SIZE);
			}

			if (_redoStack.Count > MAX_STACK_SIZE)
			{
				_redoStack.RemoveRange(0, _redoStack.Count - MAX_STACK_SIZE);
			}
		}
	}
}