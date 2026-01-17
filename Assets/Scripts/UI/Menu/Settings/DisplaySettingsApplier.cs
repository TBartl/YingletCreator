using Reactivity;
using UnityEngine;

public class DisplaySettingsApplier : ReactiveBehaviour
{
	private ISettingsManager _settingsManager;

	void Start()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		AddReflector(ReflectFpsCap);
		AddReflector(ReflectScreenSettings);
	}

	private void ReflectFpsCap()
	{
		int fpsCap = (int)_settingsManager.Settings.FpsCap;
		if (fpsCap == (int)FpsCap.VSync)
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = fpsCap;
	}

	private void ReflectScreenSettings()
	{
		var screenMode = _settingsManager.Settings.ScreenMode switch
		{
			ScreenMode.Windowed => FullScreenMode.Windowed,
			ScreenMode.Borderless => FullScreenMode.FullScreenWindow,
			ScreenMode.Fullscreen => FullScreenMode.ExclusiveFullScreen,
			_ => throw new System.Exception("Unknown screen mode"),
		};
		var size = _settingsManager.Settings.DisplayResolution;
		Screen.SetResolution(size.x, size.y, screenMode);
	}
}
