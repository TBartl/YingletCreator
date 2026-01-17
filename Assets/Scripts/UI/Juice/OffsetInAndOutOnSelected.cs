using UnityEngine;

public class OffsetInAndOutOnSelected : MonoBehaviour
{

	[SerializeField] Vector2 _entranceOffset;
	[SerializeField] Vector2 _exitOffset;
	[SerializeField] SharedEaseSettings _easeSettings;

	ISelectable _selectable;
	private RectTransform _rectTransform;
	private Vector2 _originalAnchoredPosition;
	private Coroutine _transitionCoroutine;

	private void Awake()
	{
		_selectable = this.GetComponentInParent<ISelectable>();
		_rectTransform = this.GetComponent<RectTransform>();
		_originalAnchoredPosition = _rectTransform.anchoredPosition;

		_selectable.Selected.OnChanged += Selected_OnChanged;

		if (!_selectable.Selected.Val)
		{
			Reset();
		}
	}
	private void OnDestroy()
	{
		_selectable.Selected.OnChanged -= Selected_OnChanged;
	}

	private void Selected_OnChanged(bool from, bool to)
	{
		if (to == true) this.gameObject.SetActive(true);

		Vector2 fromPos = _rectTransform.anchoredPosition;
		Vector2 toPos = _selectable.Selected.Val ? _originalAnchoredPosition : _originalAnchoredPosition + _exitOffset;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _rectTransform.anchoredPosition = Vector2.LerpUnclamped(fromPos, toPos, p), OnComplete);

		void OnComplete()
		{
			if (to == false)
			{
				Reset();
			}
		}
	}

	private void Reset()
	{
		// Move back up to the top and disable
		_rectTransform.anchoredPosition = _originalAnchoredPosition + _entranceOffset;
		this.gameObject.SetActive(false);
	}
}
