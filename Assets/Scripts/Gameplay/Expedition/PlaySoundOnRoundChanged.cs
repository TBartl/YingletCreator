using UnityEngine;

public class PlaySoundOnRoundChanged : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	private AudioPlayer _audioPlayer;
	private IExpeditionRoundManager _roundManager;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		_roundManager = this.GetExpeditionComponent<IExpeditionRoundManager>();

		_roundManager.CurrentRound.OnChanged += Round_OnChanged;
	}

	private void OnDestroy()
	{
		if (_roundManager != null)
		{
			_roundManager.CurrentRound.OnChanged -= Round_OnChanged;
		}
	}

	private void Round_OnChanged(int from, int to)
	{
		_audioPlayer.Play(_soundEffect);
	}
}