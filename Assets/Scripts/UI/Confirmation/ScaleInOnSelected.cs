using UnityEngine;

public class ScaleInOnSelected : MonoBehaviour
{
	[SerializeField] SharedEaseSettings _easeInSettings;
	[SerializeField] SharedEaseSettings _easeOutSettings;

	ISelectable _selectable;
	private RectTransform _rectTransform;
	private Coroutine _transitionCoroutine;

	private void Start()
	{
		_selectable = this.GetComponentInParent<ISelectable>();
		_rectTransform = this.GetComponent<RectTransform>();

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

		Vector3 fromScale = _rectTransform.localScale;
		Vector3 toScale = to ? Vector3.one : Vector3.zero;
		var easeSettings = to ? _easeInSettings : _easeOutSettings;
		this.StartEaseCoroutine(ref _transitionCoroutine, easeSettings, p => _rectTransform.localScale = Vector3.LerpUnclamped(fromScale, toScale, p), OnComplete);

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
		this.gameObject.SetActive(false);
	}
}
