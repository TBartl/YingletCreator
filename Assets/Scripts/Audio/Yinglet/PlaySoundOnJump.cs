using UnityEngine;

public class PlaySoundOnJump : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;

	private AudioPlayer _audioPlayer;
	private IPlayerMovement _playerMovement;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		_playerMovement = this.GetCharacterRootComponent<IPlayerMovement>();
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