using UnityEngine;
using UnityEngine.UI;

public class EnterGameOnClick : MonoBehaviour
{
	IMainMenuState _mainMenuState;
	Button _button;

	void Awake()
	{
		_mainMenuState = Singletons.GetSingleton<IMainMenuState>();

		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_mainMenuState.EnterGame();
	}
}
