using UnityEngine;

public class PlaySoundOnLeaveTitleScreen : MonoBehaviour
{

    [SerializeField] private MenuType _titleScreenMenu;
    [SerializeField] private SoundEffect _soundEffect;
    private IMenuManager _menuManager;
    private IAudioPlayer _audioPlayer;

    private void Awake()
    {
        _menuManager = Singletons.GetSingleton<IMenuManager>();
        _audioPlayer = Singletons.GetSingleton<IAudioPlayer>();

        _menuManager.OpenMenu.OnChanged += Menu_OnOpenChanged;
    }

    private void OnDestroy()
    {
        _menuManager.OpenMenu.OnChanged -= Menu_OnOpenChanged;
    }

    private void Menu_OnOpenChanged(MenuType from, MenuType to)
    {
        if (from == _titleScreenMenu && to != _titleScreenMenu)
        {
            _audioPlayer.Play(_soundEffect);
        }
    }
}
