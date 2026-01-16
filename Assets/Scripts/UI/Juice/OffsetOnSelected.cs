using Reactivity;
using UnityEngine;

// Could merge this with the other reactive behavior at some point
public class OffsetOnSelected : ReactiveBehaviour
{
	[SerializeField] Vector2 _offset;
	[SerializeField] SharedEaseSettings _easeSettings;
	private ISelectable _selectable;
	private RectTransform _rectTransform;
	private Vector2 _originalAnchoredPosition;
	private Coroutine _transitionCoroutine;

	private void Awake()
	{
		_selectable = this.GetComponentInParent<ISelectable>();
		_rectTransform = this.GetComponent<RectTransform>();
	}

	void Start()
	{
		_originalAnchoredPosition = _rectTransform.anchoredPosition;
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		Vector2 from = _rectTransform.anchoredPosition;
		Vector2 to = _selectable.Selected.Val ? _originalAnchoredPosition + _offset : _originalAnchoredPosition;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _rectTransform.anchoredPosition = Vector2.LerpUnclamped(from, to, p));
	}
}

