using UnityEngine;
using UnityEngine.UI;

internal class ToastOnClick : MonoBehaviour
{
	[SerializeField] string _text = "Button clicked";

	private IToastManager _toastManager;
	private Button _button;

	void Awake()
	{
		_toastManager = Singletons.GetSingleton<IToastManager>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_toastManager.Show(_text);
	}
}
