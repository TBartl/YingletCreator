using UnityEngine;

public class PlaySoundOnActiveCharacterChanged : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	private IAudioPlayer _audioPlayer;
	private IExpeditionCharacterManager _characterManager;

	private void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_characterManager = this.GetComponentInParentSafe<IExpeditionCharacterManager>();

		_characterManager.ActiveCharacter.OnChanged += ActiveCharacter_OnChanged;
	}

	private void OnDestroy()
	{
		_characterManager.ActiveCharacter.OnChanged -= ActiveCharacter_OnChanged;
	}

	private void ActiveCharacter_OnChanged(ExpeditionCharacter from, ExpeditionCharacter to)
	{
		if (from == null) return; // Not on first thing
		_audioPlayer.Play(_soundEffect);
	}
}
