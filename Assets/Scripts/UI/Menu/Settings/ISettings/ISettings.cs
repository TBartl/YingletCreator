using System.Collections.Generic;
using UnityEngine;

public enum FpsCap
{
	Fps30 = 30,
	Fps60 = 60,
	Fps120 = 120,
	Fps144 = 144,
	Fps240 = 240,
	Unlimited = -1,
	VSync = -2,
}

public enum ScreenMode
{
	Windowed,
	Borderless,
	Fullscreen
}

public enum UnitSystem
{
	Metric,
	Imperial,
	Kassens
}

public enum DefaultCameraPosition
{
	Static,
	Frame
}

public interface ISettings
{
	FpsCap FpsCap { get; }
	ScreenMode ScreenMode { get; }
	Vector2Int DisplayResolution { get; }
	bool UITilt { get; }
	UnitSystem UnitSystem { get; }
	DefaultCameraPosition DefaultCameraPosition { get; }
	float EffectVolume { get; }
	float MusicVolume { get; }
	List<string> DontShowConfirmationIdsAgain { get; } // Can't be a HashSet since that can't be serialized
}

public interface IWriteableSettings : ISettings
{
	new FpsCap FpsCap { get; set; }
	new ScreenMode ScreenMode { get; set; }
	new Vector2Int DisplayResolution { get; set; }
	new bool UITilt { get; set; }
	new UnitSystem UnitSystem { get; set; }
	new DefaultCameraPosition DefaultCameraPosition { get; set; }
	new float EffectVolume { get; set; }
	new float MusicVolume { get; set; }
	new List<string> DontShowConfirmationIdsAgain { get; set; }
}
