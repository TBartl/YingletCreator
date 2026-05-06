using System.Collections;
using UnityEngine;

public interface IEncounterLogPositioner
{
	void ResetPosition();
	void ObjectAdded(bool closerToBottom);
}

public class EncounterLogPositioner : MonoBehaviour, IEncounterLogPositioner, IInitializable
{
	[SerializeField] float _normalOffset = 100f;
	[SerializeField] float _closerToBottomOffset = 50f;
	[SerializeField] SharedEaseSettings _easeSettings;

	private RectTransform _rectTransform;
	private RectTransform _parentRectTransform;
	private Coroutine _coroutine;

	public void Initialize()
	{
		_rectTransform = this.GetComponentSafe<RectTransform>();
		_parentRectTransform = this.transform.parent.GetComponentSafe<RectTransform>();

	}

	public void ObjectAdded(bool closerToBottom)
	{
		CoroutineRunner.S.StopAndStartCoroutine(ref _coroutine, DelayedHandling(closerToBottom));
	}

	public void ResetPosition()
	{
		_rectTransform.anchoredPosition = new Vector2(0, -_parentRectTransform.rect.size.y); // So we come in from the bottom
	}

	IEnumerator DelayedHandling(bool closerToBottom)
	{
		// Delaying this in a coroutine in case other stuff is spawned and so we have time for the layout to be updated
		yield return null;

		var fromPosition = _rectTransform.anchoredPosition;

		var size = _rectTransform.rect.size;
		var parentSize = _parentRectTransform.rect.size;
		float y = parentSize.y - size.y - (closerToBottom ? _closerToBottomOffset : _normalOffset);
		var toPosition = new Vector2(0, -y);

		this.StartEaseCoroutine(ref _coroutine, _easeSettings, Apply);

		void Apply(float p)
		{
			_rectTransform.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, p);
		}
	}
}
