using UnityEngine;
using UnityEngine.UI;

public class PromptContinueButton : MonoBehaviour
{
	private IActiveEncounterProvider _encounterProvider;
	private Button _button;

	void Start()
	{
		_encounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();
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
		_encounterProvider.ActiveEncounter.Val.Networking.SendMessage_Continue();
	}
}
