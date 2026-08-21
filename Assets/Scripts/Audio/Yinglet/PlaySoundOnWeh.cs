using Character.Creator;
using UnityEngine;

public class PlaySoundOnWeh : MonoBehaviour
{
	[SerializeField] private SoundEffectBase _soundEffect;

	private IAudioPlayer _audioPlayer;
	private ICustomizationDataRepository _dataRepo;
	private IWehManager _wehManager;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_dataRepo = this.GetCharacterRootComponent<ICustomizationDataRepository>();
		_wehManager = this.GetComponentInParent<IWehManager>();
		_wehManager.OnWeh += OnWehTriggered;
	}

	private void OnDestroy()
	{
		_wehManager.OnWeh -= OnWehTriggered;
	}

	private void OnWehTriggered()
	{
		var options = new AudioPlayOptions
		{
			Position = transform.position,
			PitchShift = _dataRepo.CustomizationData.GenderData.VoicePitchShift.Val
		};

		_audioPlayer.Play(_soundEffect, options);
	}
}
