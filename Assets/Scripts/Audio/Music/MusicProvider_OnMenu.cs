using Reactivity;
using UnityEngine;

internal class MusicProvider_OnMenu : ReactiveBehaviour, IMusicProvider, IInitializable
{
	[SerializeField] MenuType _menuType;
	[SerializeField] AudioClip _clip;
	private IMenuManager _menuManager;
	Computed<AudioClip> _clipComputed;

	public AudioClip Clip => _clipComputed.Val;

	public void Initialize()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();

		_clipComputed = CreateComputed(ComputeClip);
	}

	private AudioClip ComputeClip()
	{
		var onMenu = _menuManager.OpenMenu.Val == _menuType;
		return onMenu ? _clip : null;
	}
}
