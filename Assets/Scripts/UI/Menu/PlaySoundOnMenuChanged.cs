using UnityEngine;

public class PlaySoundOnMenuChanged : MonoBehaviour
{

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

	private void Menu_OnOpenChanged(MenuType type1, MenuType type2)
	{
		_audioPlayer.Play(_soundEffect);
	}
}
