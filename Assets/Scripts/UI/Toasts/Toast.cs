using System.Collections;
using UnityEngine;

public class Toast : MonoBehaviour
{
	const float OffsetDistance = 50f;
	const float EaseDuration = 0.5f;
	[SerializeField] AnimationCurve _alphaEase;
	[SerializeField] AnimationCurve _positionEase;

	private CanvasGroup _canvasGroup;
	private RectTransform _rectTransform;

	void Start()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_rectTransform = GetComponent<RectTransform>();
		StartCoroutine(RunToast());
	}

	private IEnumerator RunToast()
	{
		// Fade in
		for (float t = 0; t < EaseDuration; t += Time.deltaTime)
		{
			float p = t / EaseDuration;
			_canvasGroup.alpha = _alphaEase.Evaluate(p);
			float positionY = Mathf.Lerp(-OffsetDistance, 0, _positionEase.Evaluate(p));
			_rectTransform.anchoredPosition = new Vector3(0, positionY);
			yield return null;
		}
		_rectTransform.anchoredPosition = Vector3.zero;
		_canvasGroup.alpha = 1f;

		// Wait
		yield return new WaitForSeconds(2.0f);

		for (float t = 0; t < EaseDuration; t += Time.deltaTime)
		{
			float p = t / EaseDuration;
			_canvasGroup.alpha = _alphaEase.Evaluate(1 - p);
			float positionY = Mathf.Lerp(OffsetDistance, 0, _positionEase.Evaluate(1 - p));
			_rectTransform.anchoredPosition = new Vector3(0, positionY);
			yield return null;
		}

		// Destroy the GameObject
		Destroy(gameObject);
	}
}
