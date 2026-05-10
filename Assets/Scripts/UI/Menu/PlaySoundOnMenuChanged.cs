using System;
using UnityEngine;

public interface IPlaySoundOnMenuChanged
{
	IDisposable Suspend();
}

public class PlaySoundOnMenuChanged : MonoBehaviour, IPlaySoundOnMenuChanged
{

	[SerializeField] private SoundEffect _soundEffect;
	private IMenuManager _menuManager;
	private IAudioPlayer _audioPlayer;

	int _numSuspending = 0;

	public IDisposable Suspend()
	{
		_numSuspending++;
		return new BasicActionDisposable(() => _numSuspending--);
	}

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
		if (_numSuspending > 0) return;
		_audioPlayer.Play(_soundEffect);
	}
}
