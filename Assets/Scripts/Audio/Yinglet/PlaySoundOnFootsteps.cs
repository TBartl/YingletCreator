using UnityEngine;

public class PlaySoundOnFootsteps : MonoBehaviour
{
	[SerializeField] float OFFSET = .25f;

	private AudioPlayer _audioPlayer;
	private ISurfaceSoundProvider _surfaceSoundProvider;
	private ICharacterCollisionHandling _collisionHandling;
	private IYingletAnimationBridge _animation;

	// Track which step we last played (0 or 1 for .25 and .75)
	private int _lastStepIndex = 0;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		_surfaceSoundProvider = Singletons.GetSingleton<ISurfaceSoundProvider>();
		_collisionHandling = this.GetCharacterRootComponent<ICharacterCollisionHandling>();
		_animation = this.GetCharacterRootComponent<IYingletAnimationBridge>();
	}

	void Update()
	{
		float? animTime = _animation.GetMovingAnimTime();
		if (animTime == null)
		{
			_lastStepIndex = 0;
			return;
		}

		// We want to play a step sound at .25 and .75, so we add .25 to the time and then multiply by 2 to get a number where each whole number is a step
		// Slightly adjust down from .25f to better line up with audio
		int stepIndex = Mathf.FloorToInt((animTime.Value + OFFSET) * 2);

		if (stepIndex != _lastStepIndex)
		{
			var sound = _surfaceSoundProvider.GetSound(_collisionHandling.LastGroundMaterial, SurfaceSoundType.Footstep);
			_audioPlayer.Play(sound);
			_lastStepIndex = stepIndex;
		}
	}
}