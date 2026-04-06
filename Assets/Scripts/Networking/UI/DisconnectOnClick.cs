using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectOnClick : MonoBehaviour
{
	private NetworkManager _netManager;
	private Button _button;

	void Start()
	{
		_netManager = NetworkManager.Singleton;
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_netManager.Shutdown();
	}
}