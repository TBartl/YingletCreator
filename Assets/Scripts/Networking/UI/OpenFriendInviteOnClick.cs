using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class OpenFriendInviteOnClick : MonoBehaviour
{
	private Button _button;

	void Start()
	{
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		//TODO: Implement this
		SteamFriends.OpenGameInviteOverlay(new SteamId());
	}
}
