using System;
using System.Linq;

internal class UnitSystemSettingsDropdown : MenuSettingsDropdown<UnitSystem>
{
	protected override UnitSystem Value
	{
		get => _settingsManager.Settings.UnitSystem;
		set => _settingsManager.Settings.UnitSystem = value;
	}

	protected override MenuSettingsDropdownOption[] GetAllOptions()
	{
		var enumValues = (UnitSystem[])Enum.GetValues(typeof(UnitSystem));
		var options = enumValues
			.Select(v => new MenuSettingsDropdownOption(v.ToString(), v))
			.ToArray();
		return options;
	}
}
