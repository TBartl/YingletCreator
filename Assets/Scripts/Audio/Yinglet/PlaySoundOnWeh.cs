using Character.Compositor;
using Character.Creator;
using Reactivity;
using System.Linq;
using UnityEngine;

public class PlaySoundOnWeh : ReactiveBehaviour
{
	[SerializeField] private SoundEffectBase _soundEffect;

	private IAudioPlayer _audioPlayer;
	private ICustomizationDataRepository _dataRepo;
	private ICharacterToggleProvider _toggleProvider;
	private IWehManager _wehManager;
	Computed<SoundEffectBase> _overrideSoundEffect;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_dataRepo = this.GetCharacterRootComponent<ICustomizationDataRepository>();
		_toggleProvider = this.GetCharacterRootComponent<ICharacterToggleProvider>();
		_wehManager = this.GetComponentInParent<IWehManager>();

		_overrideSoundEffect = CreateComputed(ComputeOverride);

		_wehManager.OnWeh += OnWehTriggered;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		_wehManager.OnWeh -= OnWehTriggered;
	}

	private SoundEffectBase ComputeOverride()
	{
		// Last first, so we get the most recently added
		var toggles = _toggleProvider.Toggles.Reverse().ToList();
		foreach (var toggle in toggles)
		{
			foreach (var component in toggle.Components)
			{
				if (component is OverrideWehSound wehSoundEffectComponent)
				{
					return wehSoundEffectComponent.Sound;
				}
			}
		}
		return null;
	}

	private void OnWehTriggered()
	{
		var options = new AudioPlayOptions
		{
			Position = transform.position,
			PitchShift = _dataRepo.CustomizationData.GenderData.VoicePitchShift.Val
		};
		var sound = _overrideSoundEffect.Val ?? _soundEffect;
		_audioPlayer.Play(sound, options);
	}
}
