using Reactivity;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class LowerCanvasGroupAlphaIfNotUIInteractable : ReactiveBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;
	[SerializeField] float _reducedAlpha = 0.5f;
	private CanvasGroup _canvasGroup;
	private IUIInteractable _uiInteractable;
	private Coroutine _coroutine;

	private void Start()
	{
		_canvasGroup = this.GetComponentSafe<CanvasGroup>();
		_uiInteractable = this.GetComponentSafe<IUIInteractable>();
		_uiInteractable.Interactable.OnChanged += OnInteractableChanged;

		_canvasGroup.alpha = _uiInteractable.Interactable.Val ? 1f : _reducedAlpha;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_uiInteractable != null)
		{
			_uiInteractable.Interactable.OnChanged -= OnInteractableChanged;
		}
	}

	private void OnInteractableChanged(bool from, bool to)
	{
		var fromAlpha = _canvasGroup.alpha;
		var toAlpha = to ? 1f : _reducedAlpha;
		this.StartEaseCoroutine(ref _coroutine, _easeSettings, Apply);

		void Apply(float p)
		{
			_canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, p);
		}
	}
}
