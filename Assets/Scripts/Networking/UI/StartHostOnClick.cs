using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartHostOnClick : MonoBehaviour
{
	private INetLobbyManager _lobbyManager;
	private NetworkManager _netManager;
	private Button _button;

	void Start()
	{
		_lobbyManager = Singletons.GetSingleton<INetLobbyManager>();
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
		if (_netManager.IsRunning())
		{
			Debug.LogWarning("NetworkManager is already running. Ignoring StartHost request.");
			return;
		}
		_lobbyManager.StartHostWithLobby();
	}
}
