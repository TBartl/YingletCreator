using UnityEngine;
using UnityEngine.UI;

public class OpenExpeditionCharacterPickerOnClick : MonoBehaviour
{
	private IExpeditionPlanningManager _expeditionPlanner;
	private IModalCharacterPickerManager _modalCharacterPickerManager;
	private Button _button;

	private void Awake()
	{
		_expeditionPlanner = Singletons.GetSingleton<IExpeditionPlanningManager>();
		_modalCharacterPickerManager = Singletons.GetSingleton<IModalCharacterPickerManager>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		var data = new ModalCharacterPickerData(character =>
		{
			_expeditionPlanner.AddToParty(character.CachedData);
		});
		_modalCharacterPickerManager.OpenModalCharacterPickerData(data);
	}
}
