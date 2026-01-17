using UnityEngine.Assertions;

internal class DefaultCameraPositionDropdown : MenuSettingsDropdown<DefaultCameraPosition>
{
	protected override DefaultCameraPosition Value
	{
		get => _settingsManager.Settings.DefaultCameraPosition;
		set => _settingsManager.Settings.DefaultCameraPosition = value;
	}

	protected override MenuSettingsDropdownOption[] GetAllOptions()
	{
		var options = new[]
		{
			new MenuSettingsDropdownOption("Static", DefaultCameraPosition.Static),
			new MenuSettingsDropdownOption("Frame Yinglet", DefaultCameraPosition.Frame),
		};
		Assert.AreEqual(options.Length, System.Enum.GetValues(typeof(DefaultCameraPosition)).Length, "Mismatch between defined options and enum values.");
		return options;
	}
}
