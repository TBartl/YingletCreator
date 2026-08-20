using Character.Creator;
using Networking;
using UnityEngine;

public class PlayWehOnInput : MonoBehaviour
{
	[SerializeField] private SoundEffectBase _soundEffect;

	private IAudioPlayer _audioPlayer;
	private ICharacterIdentity _playerIdentity;
	private ICustomizationDataRepository _dataRepo;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_playerIdentity = this.GetCharacterRootComponent<ICharacterIdentity>();
		_dataRepo = this.GetCharacterRootComponent<ICustomizationDataRepository>();
	}

	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Q)) return;
		if (!_playerIdentity.IsActiveAndMine) return;
		float shift = _dataRepo.CustomizationData.GenderData.VoicePitchShift.Val;
		var options = new AudioPlayOptions { Position = transform.position, PitchShift = shift };
		_audioPlayer.Play(_soundEffect, options);
	}
}
