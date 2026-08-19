using Networking;
using UnityEngine;

public class PlayWehsOnInput : MonoBehaviour
{
	[SerializeField] private SoundEffectBase _soundEffect;

	private IAudioPlayer _audioPlayer;
	private ICharacterIdentity _playerIdentity;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_playerIdentity = this.GetComponentInParentSafe<ICharacterIdentity>();
	}

	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Q)) return;
		if (!_playerIdentity.IsActiveAndMine) return;
		_audioPlayer.Play(_soundEffect, new() { Position = transform.position });
	}
}
