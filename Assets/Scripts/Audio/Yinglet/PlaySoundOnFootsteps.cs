using UnityEngine;

public class PlaySoundOnFootsteps : MonoBehaviour
{
	private IAudioPlayer _audioPlayer;
	private ISurfaceSoundProvider _surfaceSoundProvider;
	private ICharacterCollisionHandling _collisionHandling;
	private IFootstepTracker _footstepTracker;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_surfaceSoundProvider = Singletons.GetSingleton<ISurfaceSoundProvider>();
		_collisionHandling = this.GetComponentInParent<ICharacterCollisionHandling>();
		_footstepTracker = this.GetComponentInParent<IFootstepTracker>();

		_footstepTracker.OnFootstep += PlayFootstepSound;
	}

	private void OnDestroy()
	{
		_footstepTracker.OnFootstep -= PlayFootstepSound;
	}

	private void PlayFootstepSound(Vector3 pos)
	{
		var sound = _surfaceSoundProvider.GetSound(_collisionHandling.LastGroundMaterial, SurfaceSoundType.Footstep);
		_audioPlayer.Play(sound, new() { Position = pos });
	}
}