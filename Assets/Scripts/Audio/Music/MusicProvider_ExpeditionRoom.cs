using Reactivity;
using UnityEngine;

public sealed class MusicProvider_ExpeditionRoom : ReactiveBehaviour, IMusicProvider, IInitializable
{
	private ICurrentRoomProvider _roomProvider;
	Computed<AudioClip> _clipComputed;

	public AudioClip Clip => _clipComputed.Val;

	public void Initialize()
	{
		_roomProvider = Singletons.GetSingleton<ICurrentRoomProvider>();

		_clipComputed = CreateComputed(ComputeClip);
	}

	private AudioClip ComputeClip()
	{
		return _roomProvider.CurrentRoom.Val?.Music;
	}
}
