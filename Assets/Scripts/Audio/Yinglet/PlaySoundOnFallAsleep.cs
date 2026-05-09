using UnityEngine;

public class PlaySoundOnFallAsleep : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;

	private AudioPlayer _audioPlayer;
	private PlaySoundOnLand _playSoundOnLand;
	private ICharacterRoundState _roundState;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		_playSoundOnLand = this.GetComponentSafe<PlaySoundOnLand>();
		_roundState = this.GetCharacterRootComponent<ICharacterRoundState>();
		_roundState.IsAsleep.OnChanged += IsAsleep_OnChanged;
	}

	private void OnDestroy()
	{
		if (_roundState != null)
		{
			_roundState.IsAsleep.OnChanged -= IsAsleep_OnChanged;
		}
	}

	private void IsAsleep_OnChanged(bool from, bool to)
	{
		if (to)
		{
			_audioPlayer.Play(_soundEffect, new() { Position = transform.position });
			_playSoundOnLand.ForcePlay(5);
		}
	}
}