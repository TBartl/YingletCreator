using UnityEngine;
using UnityEngine.UI;

public class CopyLobbyToClipboardOnClick : MonoBehaviour
{
	private INetStateReader _netState;
	private Button _button;

	void Start()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		var currentLobby = _netState.CurrentLobby;
		if (currentLobby == null)
		{
			Debug.LogWarning("Tried to copy lobby ID to clipboard, but there is no current lobby.");
			return;
		}
		ulong lobbyId = currentLobby.Value.Id.Value;
		GUIUtility.systemCopyBuffer = lobbyId.ToString();
	}
}
