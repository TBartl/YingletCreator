public abstract class MenuSettingsDropdown<T> : ReactiveDropdown<T>
{
	protected ISettingsManager _settingsManager;

	protected override void Awake()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		base.Awake();
	}

	protected override void Dropdown_OnValueChanged(int index)
	{
		base.Dropdown_OnValueChanged(index);
		_settingsManager.SaveChangesToDisk();
	}

}
