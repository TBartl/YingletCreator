using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Character.Creator.UI
{
	public class SecretExtendSlidersOnMultiClick : MonoBehaviour, IPointerClickHandler
	{
		string _originalText;
		const int SWAP_CLICKS = 5;
		int _clicks = 0;
		private TextMeshProUGUI _text;

		void Awake()
		{
			_text = this.GetComponent<TextMeshProUGUI>();
			_originalText = _text.text;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_clicks += 1;
			if (_clicks == SWAP_CLICKS)
			{
				SetSliders(false);
				_text.text = "GIGA SLIDERS";
			}
			else if (_clicks > SWAP_CLICKS)
			{
				SetSliders(true);
				_clicks = 0;
				_text.text = _originalText;
			}
		}

		void SetSliders(bool isExtended)
		{
			var canvas = this.GetComponentInParent<Canvas>();
			var sliders = canvas.GetComponentsInChildren<IExtendableSlider>();
			foreach (var slider in sliders)
			{
				slider.SetExtended(isExtended);
			}
		}
	}

	public interface IExtendableSlider
	{
		void SetExtended(bool isExtended);
	}

}
