using Reactivity;
using UnityEngine;

public interface ISettingsManager
{
	IWriteableSettings Settings { get; }

	void SaveChangesToDisk();
}
public class SettingsManager : MonoBehaviour, ISettingsManager
{
	private ISettingsDiskIO _settingsIO;

	Observable<IWriteableSettings> _settings = new();

	public IWriteableSettings Settings => _settings.Val;

	private void Awake()
	{
		_settingsIO = Singletons.GetSingleton<ISettingsDiskIO>();
		var loadedSettings = _settingsIO.LoadSettings();
		_settings.Val = new ObservableSettings(loadedSettings);
	}

	public void SaveChangesToDisk()
	{
		_settingsIO.SaveSettings(_settings.Val);
	}
}
