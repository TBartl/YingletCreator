using System.Collections;
using UnityEngine;

public class GameMusicPlayer : MonoBehaviour
{
	[SerializeField] AudioClip _natureAmbienceClip;

	private IAudioMixerProvider _mixerProvider;
	private AudioSource _source;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_mixerProvider = Singletons.GetSingleton<IAudioMixerProvider>();

		_source = this.gameObject.AddComponent<AudioSource>();
		_source.clip = _natureAmbienceClip;
		_source.loop = true;
		_source.outputAudioMixerGroup = _mixerProvider.MusicGroup;
		_source.Stop();

		StartCoroutine(StartAfterDelay());
	}

	private void OnDestroy()
	{
		Destroy(_source);
	}
	private IEnumerator StartAfterDelay()
	{
		yield return null;

		_source.Play();
	}
}
