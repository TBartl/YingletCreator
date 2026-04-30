using Reactivity;
using System.Collections;
using UnityEngine;

public interface IMusicProvider
{
	AudioClip Clip { get; }
}

public class MusicPlayer : ReactiveBehaviour
{
	[SerializeField] float CROSSFADE_DURATION = .3f;

	private IAudioMixerProvider _mixerProvider;
	private IMusicProvider[] _providers;
	private AudioSource _mainSource;
	private AudioSource _backupSource;

	Computed<AudioClip> _currentClip;
	private Coroutine _coroutine;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_mixerProvider = Singletons.GetSingleton<IAudioMixerProvider>();

		_providers = this.GetComponentsSafe<IMusicProvider>();

		_mainSource = CreateSource();
		_backupSource = CreateSource();

		_currentClip = CreateComputed(ComputeCurrentClip);
		_currentClip.OnChanged += Clip_OnChanged;
		Clip_OnChanged(null, _currentClip.Val);
	}

	AudioSource CreateSource()
	{
		var source = this.gameObject.AddComponent<AudioSource>();
		source.loop = true;
		source.outputAudioMixerGroup = _mixerProvider.MusicGroup;
		source.Stop();
		return source;
	}

	private AudioClip ComputeCurrentClip()
	{
		foreach (var provider in _providers)
		{
			if (provider.Clip != null)
			{
				return provider.Clip;
			}
		}
		return null;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		Destroy(_mainSource);
		Destroy(_backupSource);
	}

	private IEnumerator SwapToClip(AudioClip newClip)
	{
		// Swap
		var newMainSource = _backupSource;
		_backupSource = _mainSource;
		_mainSource = newMainSource;

		if (newClip != null)
		{
			_mainSource.clip = newClip;
			_mainSource.time = Time.timeSinceLevelLoad % newClip.length; // Pick up where we'd leave off
			_mainSource.volume = 0;
			_mainSource.Play();
		}

		var backupOriginalVolume = _backupSource.volume;

		for (float t = 0; t < CROSSFADE_DURATION; t += Time.deltaTime)
		{
			float p = t / CROSSFADE_DURATION;
			if (_mainSource.clip != null)
			{

			}
			_mainSource.volume = Mathf.Lerp(0, 1, p);
			if (_backupSource.clip != null)
			{
				_backupSource.volume = Mathf.Lerp(backupOriginalVolume, 0, p);
			}
			yield return null;
		}

		_mainSource.volume = 1;
		_backupSource.Stop();
		_backupSource.clip = null;
	}

	private void Clip_OnChanged(AudioClip from, AudioClip to)
	{
		this.StopAndStartCoroutine(ref _coroutine, SwapToClip(to));
	}
}
