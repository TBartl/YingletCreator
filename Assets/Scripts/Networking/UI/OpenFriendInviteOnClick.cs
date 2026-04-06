using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class OpenFriendInviteOnClick : MonoBehaviour
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
			Debug.LogWarning("Tried to open friend invite overlay but not currently in a lobby");
			return;
		}
		SteamFriends.OpenGameInviteOverlay(currentLobby.Value.Id);
	}
}
