
using Reactivity;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObservableSettings : IWriteableSettings
{
	Observable<FpsCap> _fpsCap;
	public FpsCap FpsCap { get => _fpsCap.Val; set => _fpsCap.Val = value; }

	Observable<ScreenMode> _screenMode;
	public ScreenMode ScreenMode { get => _screenMode.Val; set => _screenMode.Val = value; }

	Observable<Vector2Int> _displayResolution;
	public Vector2Int DisplayResolution { get => _displayResolution.Val; set => _displayResolution.Val = value; }

	Observable<bool> _uiTilt;
	public bool UITilt { get => _uiTilt.Val; set => _uiTilt.Val = value; }

	Observable<UnitSystem> _unitSystem;
	public UnitSystem UnitSystem { get => _unitSystem.Val; set => _unitSystem.Val = value; }

	Observable<DefaultCameraPosition> _defaultCameraPosition;
	public DefaultCameraPosition DefaultCameraPosition { get => _defaultCameraPosition.Val; set => _defaultCameraPosition.Val = value; }

	Observable<float> _effectVolume;
	public float EffectVolume { get => _effectVolume.Val; set => _effectVolume.Val = value; }

	Observable<float> _musicVolume;
	public float MusicVolume { get => _musicVolume.Val; set => _musicVolume.Val = value; }

	List<string> _dontShowConfirmationIdsAgain = new(); // Doesn't actually need to be observable, keep it simple
	public List<string> DontShowConfirmationIdsAgain { get => _dontShowConfirmationIdsAgain; set => _dontShowConfirmationIdsAgain = value; }

	public ObservableSettings(ISettings loadedSettings)
	{
		_fpsCap = new Observable<FpsCap>(loadedSettings.FpsCap);
		_screenMode = new Observable<ScreenMode>(loadedSettings.ScreenMode);
		_displayResolution = new Observable<Vector2Int>(loadedSettings.DisplayResolution);
		_uiTilt = new Observable<bool>(loadedSettings.UITilt);
		_unitSystem = new Observable<UnitSystem>(loadedSettings.UnitSystem);
		_defaultCameraPosition = new Observable<DefaultCameraPosition>(loadedSettings.DefaultCameraPosition);
		_effectVolume = new Observable<float>(loadedSettings.EffectVolume);
		_musicVolume = new Observable<float>(loadedSettings.MusicVolume);
		_dontShowConfirmationIdsAgain = loadedSettings.DontShowConfirmationIdsAgain;
	}
}
