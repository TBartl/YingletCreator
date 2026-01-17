using UnityEngine.Assertions;

internal class ScreenModeSettingsDropdown : MenuSettingsDropdown<ScreenMode>
{
	protected override ScreenMode Value
	{
		get => _settingsManager.Settings.ScreenMode;
		set => _settingsManager.Settings.ScreenMode = value;
	}

	protected override MenuSettingsDropdownOption[] GetAllOptions()
	{
		var options = new[]
		{
			new MenuSettingsDropdownOption("Windowed", ScreenMode.Windowed),
			new MenuSettingsDropdownOption("Borderless", ScreenMode.Borderless),
			new MenuSettingsDropdownOption("Fullscreen", ScreenMode.Fullscreen)
		};
		Assert.AreEqual(options.Length, System.Enum.GetValues(typeof(ScreenMode)).Length, "Mismatch between defined options and enum values.");
		return options;
	}
}
