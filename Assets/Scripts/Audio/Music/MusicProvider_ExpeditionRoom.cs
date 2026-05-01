using Reactivity;
using UnityEngine;

public sealed class MusicProvider_ExpeditionRoom : ReactiveBehaviour, IMusicProvider, IInitializable
{
	private IActiveRoomProvider _roomProvider;
	Computed<AudioClip> _clipComputed;

	public AudioClip Clip => _clipComputed.Val;

	public void Initialize()
	{
		_roomProvider = Singletons.GetSingleton<IActiveRoomProvider>();

		_clipComputed = CreateComputed(ComputeClip);
	}

	private AudioClip ComputeClip()
	{
		return _roomProvider.ActiveRoom.Val?.Music;
	}
}
