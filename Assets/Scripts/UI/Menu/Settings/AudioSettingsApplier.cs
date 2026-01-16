using Reactivity;
using UnityEngine;

public class AudioSettingsApplier : ReactiveBehaviour
{
	private IAudioMixerProvider _mixerProvider;
	private ISettingsManager _settingsManager;

	void Start()
	{
		_mixerProvider = Singletons.GetSingleton<IAudioMixerProvider>();
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		AddReflector(ReflectSoundEffectsVolume);
		AddReflector(ReflectMusicVolume);
	}

	private void ReflectSoundEffectsVolume()
	{
		var volume = PrepareVolume(_settingsManager.Settings.EffectVolume);
		_mixerProvider.Mixer.SetFloat("SoundEffectsVolume", volume);
	}

	private void ReflectMusicVolume()
	{
		var volume = PrepareVolume(_settingsManager.Settings.MusicVolume);
		_mixerProvider.Mixer.SetFloat("MusicVolume", volume);
	}

	float PrepareVolume(float inputPercent)
	{
		float percent = Mathf.Clamp01(inputPercent);

		if (percent <= 0.001f)
			return -80f;

		return Mathf.Log10(percent) * 20;
	}
}
