using System.Linq;
using UnityEngine;

internal class WindowSizeSettingsDropdown : MenuSettingsDropdown<Vector2Int>
{
    protected override Vector2Int Value
    {
        get => _settingsManager.Settings.DisplayResolution;
        set => _settingsManager.Settings.DisplayResolution = value;
    }

    protected override MenuSettingsDropdownOption[] GetAllOptions()
    {
        return Screen.resolutions
            .Select(res => new Vector2Int(res.width, res.height))
            .Distinct()
            .Select(vec => new MenuSettingsDropdownOption($"{vec.x}x{vec.y}", vec))
            .ToArray();
    }
}
