using UnityEngine;
using UnityEngine.UI;

public class CancelConfirmationOnClick : MonoBehaviour
{
	private IConfirmationManager _confirmationManager;
	private Button _button;

	void Start()
	{
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_confirmationManager.Cancel();
	}
}
