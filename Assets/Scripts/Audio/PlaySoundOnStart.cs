using UnityEngine;

public class PlaySoundOnStart : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	[SerializeField] private bool _playOnPosition;

	private AudioPlayer _audioPlayer;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		if (_playOnPosition)
		{
			_audioPlayer.Play(_soundEffect, new() { Position = transform.position });
		}
		else
		{
			_audioPlayer.Play(_soundEffect);
		}
	}
}