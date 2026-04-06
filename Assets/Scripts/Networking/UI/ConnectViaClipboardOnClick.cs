using UnityEngine;
using UnityEngine.UI;

public class ConnectViaClipboardOnClick : MonoBehaviour
{
	private INetStateWriter _netState;
	private IToastManager _toastManager;
	private Button _button;

	void Start()
	{
		_netState = Singletons.GetSingleton<INetStateWriter>();
		_toastManager = Singletons.GetSingleton<IToastManager>();
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
			Debug.LogWarning("NetworkManager is already running. Ignoring Connect request.");
			return;
		}

		var clipboardString = GUIUtility.systemCopyBuffer;

		if (!ulong.TryParse(clipboardString.Trim(), out ulong lobbyId))
		{
			Debug.LogWarning($"Clipboard string '{clipboardString}' is not a valid lobby ID.");
			_toastManager.Show("Clipboard does not contain a valid lobby ID.");
			return;
		}

		_netState.StartConnectToLobby(lobbyId);
	}
}
