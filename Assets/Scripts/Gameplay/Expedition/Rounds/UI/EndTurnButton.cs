using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
	private IGlobalRoundProvider _globalRoundProvider;
	private Button _button;

	void Start()
	{
		_globalRoundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		_button = GetComponent<Button>();

		_button.onClick.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		if (_button == null) return;
		_button.onClick.RemoveListener(OnClick);
	}

	private void OnClick()
	{
		var activeCharacter = _globalRoundProvider.ActiveCharacterState;
		if (activeCharacter == null) return;

		if (!activeCharacter.IsAsleep.Val)
		{
			activeCharacter.GoToSleep();
		}
		else
		{
			activeCharacter.WakeBackUp();
		}
	}
}
