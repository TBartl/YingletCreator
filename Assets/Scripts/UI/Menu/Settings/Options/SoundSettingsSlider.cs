using Reactivity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SoundSettingsSliderType
{
	EffectVolume,
	MusicVolume
}

public class SliderSoundSettings : ReactiveBehaviour, IPointerUpHandler
{
	[SerializeField] private SoundSettingsSliderType _sliderType = SoundSettingsSliderType.EffectVolume;

	private ISettingsManager _settingsManager;
	private Slider _slider;

	private void Awake()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		_slider = this.GetComponentInChildren<Slider>();
		_slider.onValueChanged.AddListener(Slider_OnValueChanged);
	}

	private void Start()
	{
		AddReflector(ReflectSliderValue);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		_slider.onValueChanged.RemoveListener(Slider_OnValueChanged);
	}

	private void ReflectSliderValue()
	{
		_slider.SetValueWithoutNotify(Volume);
	}
	private void Slider_OnValueChanged(float arg0)
	{
		Volume = arg0;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_settingsManager.SaveChangesToDisk();
	}

	float Volume
	{
		get
		{
			switch (_sliderType)
			{
				case SoundSettingsSliderType.EffectVolume:
					return _settingsManager.Settings.EffectVolume;
				case SoundSettingsSliderType.MusicVolume:
					return _settingsManager.Settings.MusicVolume;
				default:
					throw new System.ArgumentOutOfRangeException();
			}
		}
		set
		{
			switch (_sliderType)
			{
				case SoundSettingsSliderType.EffectVolume:
					_settingsManager.Settings.EffectVolume = value;
					break;
				case SoundSettingsSliderType.MusicVolume:
					_settingsManager.Settings.MusicVolume = value;
					break;
				default:
					throw new System.ArgumentOutOfRangeException();
			}
		}
	}
}

