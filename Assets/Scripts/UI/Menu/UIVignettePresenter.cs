using UnityEngine;
using UnityEngine.UI;

public class UIVignettePresenter : MonoBehaviour
{
	[SerializeField] private SharedEaseSettings _easeSettings;

	private Coroutine _coroutine;
	private IUIVignetteManager _uiVignetteManager;
	private Image _image;

	void Start()
	{
		_uiVignetteManager = Singletons.GetSingleton<IUIVignetteManager>();
		_image = this.GetComponent<Image>();

		_image.enabled = false;

		_uiVignetteManager.OnFlashVignette += FlashVignette;
	}

	private void OnDestroy()
	{
		_uiVignetteManager.OnFlashVignette -= FlashVignette;
	}

	private void FlashVignette(Color toColor)
	{
		Color fromColor = new Color(toColor.r, toColor.g, toColor.b, 0f);
		_image.enabled = true;
		this.StartEaseCoroutine(ref _coroutine, _easeSettings, Apply, OnComplete);

		void Apply(float p)
		{
			_image.color = Color.LerpUnclamped(fromColor, toColor, p);
		}
		void OnComplete()
		{
			_image.enabled = false;
		}
	}

}
