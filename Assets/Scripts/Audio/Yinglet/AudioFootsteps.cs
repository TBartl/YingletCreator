using UnityEngine;

public class AudioFootsteps : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	[SerializeField] float OFFSET = .25f;

	private AudioPlayer _audioPlayer;
	private IYingletAnimationBridge _animation;

	// Track which step we last played (0 or 1 for .25 and .75)
	private int _lastStepIndex = 0;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
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
		Debug.Log($"Anim time: {animTime}, step index: {stepIndex}");


		if (stepIndex != _lastStepIndex)
		{
			_audioPlayer.Play(_soundEffect);
			_lastStepIndex = stepIndex;
		}
	}
}