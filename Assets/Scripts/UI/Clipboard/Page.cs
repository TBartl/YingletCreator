using Reactivity;
using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class Page : ReactiveBehaviour
	{
		[SerializeField] Image _tintImage;
		[SerializeField] Color _startTintColor;
		[SerializeField] EaseSettings _untintEaseSettings;

		private IClipboardOrdering _clipboardOrdering;
		private IClipboardFreeFallManager _freeFallManager;

		private Vector3 _originalPos;
		private Quaternion _originalRot;
		private CanvasGroup _canvasGroup;
		private ISelectable _elementSelection;
		private Coroutine _tintCoroutine;

		void Awake()
		{
			_originalPos = this.transform.localPosition;
			_originalRot = this.transform.localRotation;
			_canvasGroup = this.GetComponent<CanvasGroup>();
			_clipboardOrdering = this.GetComponentInParent<IClipboardOrdering>();
			_freeFallManager = this.GetComponentInParent<IClipboardFreeFallManager>();
			_elementSelection = this.GetComponent<ISelectable>();
			_tintImage.color = Color.clear;
			_tintImage.gameObject.SetActive(false);
			this.gameObject.SetActive(false);
			AddReflector(ReflectSelected);
		}
		private void OnEnable()
		{
			// The coroutine may have stopped when the whole clipboard was disabled
			// Reset the color incase we were mid-tint
			_tintImage.color = Color.clear;
		}

		private void ReflectSelected()
		{
			bool isSelected = _elementSelection.Selected.Val;
			_canvasGroup.interactable = isSelected;

			_clipboardOrdering.SendToLayer(this.transform, isSelected ? ClipboardLayer.ActivePage : ClipboardLayer.Back);

			if (isSelected)
			{
				this.gameObject.SetActive(true);
				ResetTransform();

				_tintImage.gameObject.SetActive(true);
				this.StartEaseCoroutine(ref _tintCoroutine, _untintEaseSettings,
					p => _tintImage.color = Color.Lerp(_startTintColor, Color.clear, p),
					() => _tintImage.gameObject.SetActive(false));
			}
			else
			{
				_freeFallManager.FreeFall(this.transform);
			}
		}

		void ResetTransform()
		{
			this.transform.localPosition = _originalPos;
			this.transform.localRotation = _originalRot;
		}
	}
}