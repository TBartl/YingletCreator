using Character.Creator.UI;
using UnityEngine;
using UnityEngine.UI;

public class PickCharacterOnClick : MonoBehaviour
{
	private IPortraitReference _reference;
	private IModalCharacterPickerManager _modalCharacterPickerManager;
	private Button _button;

	private void Awake()
	{
		_modalCharacterPickerManager = Singletons.GetSingleton<IModalCharacterPickerManager>();
		_reference = this.GetComponent<IPortraitReference>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_modalCharacterPickerManager.PickForCurrent(_reference.Reference);
	}
}
