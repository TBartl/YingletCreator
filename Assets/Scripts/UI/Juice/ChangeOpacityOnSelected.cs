using Reactivity;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ChangeOpacityOnSelected : ReactiveBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;
	private ISelectable _selectable;
	private CanvasGroup _canvasGroup;
	private Coroutine _transitionCoroutine;

	private void Awake()
	{
		_selectable = this.GetComponentInParent<ISelectable>();
		_canvasGroup = this.GetComponent<CanvasGroup>();
	}

	void Start()
	{
		_canvasGroup.alpha = _selectable.Selected.Val ? 1 : 0;
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		float from = _canvasGroup.alpha;
		float to = _selectable.Selected.Val ? 1 : 0;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _canvasGroup.alpha = Mathf.LerpUnclamped(from, to, p));
	}
}

