using UnityEngine;
using UnityEngine.UI;

public class StartExpeditionOnClick : MonoBehaviour
{
	private IExpeditionManager _expeditionManager;
	private Button _button;

	void Start()
	{
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();
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
		_expeditionManager.StartExpedition();
	}
}
