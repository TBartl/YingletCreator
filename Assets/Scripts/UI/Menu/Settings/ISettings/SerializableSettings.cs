using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class SerializableSettings : ISettings
{
	[SerializeField] FpsCap _fpsCap = FpsCap.Fps120;
	public FpsCap FpsCap { get => _fpsCap; }

	[SerializeField] ScreenMode _screenMode = ScreenMode.Borderless;
	public ScreenMode ScreenMode { get => _screenMode; }

	[SerializeField] Vector2Int _displayResolution;
	public Vector2Int DisplayResolution { get => _displayResolution; }

	[SerializeField] bool _uiTilt = true;
	public bool UITilt => _uiTilt;

	[SerializeField] UnitSystem _unitSystem = UnitSystem.Metric;
	public UnitSystem UnitSystem => _unitSystem;

	[SerializeField] DefaultCameraPosition _defaultCameraPosition = DefaultCameraPosition.Static;
	public DefaultCameraPosition DefaultCameraPosition => _defaultCameraPosition;

	[SerializeField] float _effectVolume = 1.0f;
	public float EffectVolume => _effectVolume;

	[SerializeField] float _musicVolume = 0.42f;
	public float MusicVolume => _musicVolume;

	[SerializeField] List<string> _dontShowConfirmationIdsAgain = new();
	public List<string> DontShowConfirmationIdsAgain => _dontShowConfirmationIdsAgain;

	public SerializableSettings()
	{
		if (_displayResolution == Vector2Int.zero)
		{
			_displayResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
		}
	}

	public SerializableSettings(ISettings settings)
	{
		_fpsCap = settings.FpsCap;
		_screenMode = settings.ScreenMode;
		_displayResolution = settings.DisplayResolution;
		_uiTilt = settings.UITilt;
		_unitSystem = settings.UnitSystem;
		_defaultCameraPosition = settings.DefaultCameraPosition;
		_effectVolume = settings.EffectVolume;
		_musicVolume = settings.MusicVolume;
		_dontShowConfirmationIdsAgain = settings.DontShowConfirmationIdsAgain;
	}
}
