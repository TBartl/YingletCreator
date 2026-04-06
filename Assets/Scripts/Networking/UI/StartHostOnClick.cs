using UnityEngine;
using UnityEngine.UI;

public class StartHostOnClick : MonoBehaviour
{
	private INetStateWriter _netState;
	private Button _button;

	void Start()
	{
		_netState = Singletons.GetSingleton<INetStateWriter>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		if (_netState.IsInAnyState)
		{
			Debug.LogWarning("NetworkManager is already running. Ignoring StartHost request.");
			return;
		}
		_netState.StartHost();
	}
}
