

using Reactivity;
using UnityEngine;

internal class MusicProvider_InLobby : ReactiveBehaviour, IMusicProvider, IInitializable
{
	[SerializeField] AudioClip _clip;

	Computed<AudioClip> _clipComputed;
	private IExpeditionManager _expeditionManager;

	public AudioClip Clip => _clipComputed.Val;

	public void Initialize()
	{
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();

		_clipComputed = CreateComputed(ComputeClip);
	}

	private AudioClip ComputeClip()
	{
		var inLobby = _expeditionManager.State.Val == ExpeditionState.Planning;
		return inLobby ? _clip : null;
	}
}
