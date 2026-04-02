using UnityEngine;

public class PlaySoundOnJump : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;

	private AudioPlayer _audioPlayer;
	private ICharacterMovement _playerMovement;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		_playerMovement = this.GetCharacterRootComponent<ICharacterMovement>();
		_playerMovement.OnJump += OnJump;
	}

	private void OnDestroy()
	{
		_playerMovement.OnJump -= OnJump;
	}

	private void OnJump(Vector3 vector)
	{
		_audioPlayer.Play(_soundEffect);
	}
}