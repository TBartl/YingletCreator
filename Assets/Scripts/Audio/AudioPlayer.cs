using System.Collections.Generic;
using UnityEngine;

public interface IAudioPlayer
{
	AudioSource Play(ISoundEffect soundEffect);
	AudioSource Play(ISoundEffect soundEffect, AudioPlayOptions options);
}

[DefaultExecutionOrder(50000)] // Make huge, this should be the last to run due to the LateUpdate
public class AudioPlayer : MonoBehaviour, IAudioPlayer, IInitializable
{
	private IAudioMixerProvider _mixerProvider;

	HashSet<ISoundEffect> _playedThisFrame = new();

	public void Initialize()
	{
		_mixerProvider = Singletons.GetSingleton<IAudioMixerProvider>();
	}

	private void LateUpdate()
	{
		_playedThisFrame.Clear();
	}

	public AudioSource Play(ISoundEffect soundEffect)
	{
		return Play(soundEffect, new AudioPlayOptions());
	}

	public AudioSource Play(ISoundEffect soundEffect, AudioPlayOptions options)
	{
		if (_playedThisFrame.Contains(soundEffect))
		{
			// If something is spamming this, we don't want to blow stuff up
			// This was mainly implemented because the transition woosh was happening from a few places on Expedition start
			return null;
		}

		var go = new GameObject(soundEffect.Name);
		var source = go.AddComponent<AudioSource>();
		source.clip = soundEffect.Clip;
		source.loop = false;
		source.volume = Mathf.Min(1, soundEffect.Volume * options.Volume);
		source.pitch = Random.Range(soundEffect.RandomPitchRange.x, soundEffect.RandomPitchRange.y);
		source.outputAudioMixerGroup = _mixerProvider.SoundEffectsGroup;

		if (options.Position.HasValue)
		{
			go.transform.position = options.Position.Value;
			source.spatialBlend = 1;
			source.rolloffMode = AudioRolloffMode.Linear;
			source.maxDistance = 10;
			source.dopplerLevel = 0; // Doppler effect sounds cringe
		}
		else
		{
			// We only want to restrict non-positioned sound effects to one-per-frame
			_playedThisFrame.Add(soundEffect);
		}

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

	public Vector3? Position { get; set; } = null;
}