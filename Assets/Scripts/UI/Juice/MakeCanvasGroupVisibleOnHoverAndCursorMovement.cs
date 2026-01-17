using Character.Creator.UI;
using Reactivity;
using UnityEngine;

public class MakeCanvasGroupVisibleOnHoverAndCursorMovement : ReactiveBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;
	private IUiHoverManager _uiHoverManager;
	private IPhotoModeState _photoModeState;
	private CanvasGroup _canvasGroup;
	private Coroutine _transitionCoroutine;
	private Observable<bool> _cursorMovedRecently = new(false);
	private Computed<bool> _shouldBeVisible;
	private Vector3 _lastMousePosition;
	private float _lastMovementTime;
	const float CURSOR_MOVE_TIME = 1.8f;
	const float CURSOR_MOVE_TIME_NO_UI = 0.5f;

	private void Awake()
	{
		_uiHoverManager = Singletons.GetSingleton<IUiHoverManager>();
		_photoModeState = this.GetComponentInParent<IPhotoModeState>();
		_canvasGroup = this.GetComponent<CanvasGroup>();
		_canvasGroup.alpha = 0;
	}

	void Start()
	{
		_shouldBeVisible = this.CreateComputed(ComputeShouldBeVisible);
		AddReflector(Reflect);
	}

	void Update()
	{
		Vector3 currentMousePosition = Input.mousePosition;

		if (currentMousePosition != _lastMousePosition)
		{
			_lastMovementTime = Time.time;
			_lastMousePosition = currentMousePosition;
		}

		// Hide instructions slightly faster if we're in photo mode
		float moveTime = (_photoModeState.IsInPhotoMode.Val) ? CURSOR_MOVE_TIME_NO_UI : CURSOR_MOVE_TIME;
		_cursorMovedRecently.Val = (Time.time - _lastMovementTime) <= moveTime;
	}

	private bool ComputeShouldBeVisible()
	{
		return !_uiHoverManager.HoveringUi && _cursorMovedRecently.Val;
	}

	private void Reflect()
	{
		float from = _canvasGroup.alpha;
		float to = _shouldBeVisible.Val ? 1 : 0;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _canvasGroup.alpha = Mathf.LerpUnclamped(from, to, p));
	}
}
