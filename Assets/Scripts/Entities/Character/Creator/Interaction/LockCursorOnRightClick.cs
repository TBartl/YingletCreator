using UnityEngine;
using UnityEngine.InputSystem;

public class LockCursorOnRightClick : MonoBehaviour
{
	private Vector2 _lastCursorPosition;
	private bool _wasLocked = false;
	private ICharacterCreatorTracker _characterCreatorTracker;

	private void Start()
	{
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
	}

	private void Update()
	{
		if (Mouse.current == null) return;

		bool shouldLock = ShouldLock();

		Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !shouldLock;

		if (shouldLock && !_wasLocked)
		{
			// Store the cursor position before locking
			_lastCursorPosition = Mouse.current.position.ReadValue();
		}

		if (!shouldLock && _wasLocked)
		{
			// Restore the cursor position when unlocking
			Mouse.current.WarpCursorPosition(_lastCursorPosition);
		}
		_wasLocked = shouldLock;
	}

	bool ShouldLock()
	{
		if (!_characterCreatorTracker.IsInCharacterCreator.Val) return false;
		return Input.GetMouseButton(1);
	}

	private void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
