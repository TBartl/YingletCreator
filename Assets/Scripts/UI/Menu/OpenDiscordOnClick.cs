using UnityEngine;
using UnityEngine.UI;

public class OpenDiscordOnClick : MonoBehaviour
{
    const string DiscordUrl = "https://www.discord.gg/oops-all-yinglets";

    private Button _button;

    void Awake()
    {
        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(Button_OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(Button_OnClick);
    }

    private void Button_OnClick()
    {
        Application.OpenURL(DiscordUrl);
    }
}
