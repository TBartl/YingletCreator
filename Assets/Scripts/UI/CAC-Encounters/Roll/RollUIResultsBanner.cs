using Encounters.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollUIResultsBanner : MonoBehaviour
{
	[SerializeField] RollUISettings _settings;
	[SerializeField] SharedEaseSettings _alphaEase;
	[SerializeField] SharedEaseSettings _textMoveEase;
	[SerializeField] float _textMoveAmount = 40f;
	[SerializeField] Image _bannerImage;
	[SerializeField] TMP_Text _bannerText;
	private CanvasGroup _canvasGroup;
	private RollNodeVisitData _data;
	private Coroutine _alphaCoroutine;
	private Coroutine _textMoveCoroutine;

	private void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_canvasGroup = this.GetComponentSafe<CanvasGroup>();
		_data = reference.Record.VisitData as RollNodeVisitData;
		_data.StateObservable.OnChanged += OnRollStateChanged;
		_bannerImage.enabled = false;
		_bannerText.enabled = false;
	}
	private void OnDestroy()
	{
		if (_data != null)
		{
			_data.StateObservable.OnChanged -= OnRollStateChanged;
		}
	}

	private void OnRollStateChanged(RollState from, RollState to)
	{
		if (to != RollState.ShowingResult) return;

		this.gameObject.SetActive(true);
		var settings = _settings.RollClassificationColorMap[_data.RealBranch.Classification];
		_bannerImage.color = settings.BannerColor;
		_bannerText.text = settings.BannerText;
		_bannerImage.enabled = true;
		_bannerText.enabled = true;

		this.StartEaseCoroutine(ref _alphaCoroutine, _alphaEase, p =>
		{
			_canvasGroup.alpha = Mathf.Lerp(0f, 1f, p);
		}, () =>
		{
			_bannerImage.enabled = false;
			_bannerText.enabled = false;
		});
		this.StartEaseCoroutine(ref _textMoveCoroutine, _textMoveEase, p =>
		{
			_bannerText.rectTransform.anchoredPosition = Vector2.LerpUnclamped(Vector2.zero, new Vector2(_textMoveAmount, 0), p);
		});

		//void Apply(float p)
		//{
		//	this.transform.localScale = Vector3.one * Mathf.Lerp(1f, _scale, p);
		//}
		//void OnComplete()
		//{
		//	this.transform.localScale = Vector3.one;
		//}
	}
}
