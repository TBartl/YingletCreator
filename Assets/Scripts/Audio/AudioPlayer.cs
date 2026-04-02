using UnityEngine;

public interface IAudioPlayer
{
	AudioSource Play(ISoundEffect soundEffect);
	AudioSource Play(ISoundEffect soundEffect, AudioPlayOptions options);
}

public class AudioPlayer : MonoBehaviour, IAudioPlayer
{
	private IAudioMixerProvider _mixerProvider;

	private void Awake()
	{
		_mixerProvider = Singletons.GetSingleton<IAudioMixerProvider>();
	}
	public AudioSource Play(ISoundEffect soundEffect)
	{
		return Play(soundEffect, new AudioPlayOptions());
	}

	public AudioSource Play(ISoundEffect soundEffect, AudioPlayOptions options)
	{
		var go = new GameObject(soundEffect.Name);
		var source = go.AddComponent<AudioSource>();
		source.clip = soundEffect.Clip;
		source.loop = false;
		source.volume = Mathf.Min(1, soundEffect.Volume * options.Volume);
		source.pitch = Random.Range(soundEffect.RandomPitchRange.x, soundEffect.RandomPitchRange.y);
		source.outputAudioMixerGroup = _mixerProvider.SoundEffectsGroup;
		source.Play();

		if (options.AutoDestroy)
		{
			GameObject.Destroy(go, soundEffect.Clip.length + .25f);
		}
		return source;
	}
}

public class AudioPlayOptions
{
	/// <summary>
	/// Default true
	/// </summary>
	public bool AutoDestroy { get; set; } = true;

	public float Volume { get; set; } = 1f;
}