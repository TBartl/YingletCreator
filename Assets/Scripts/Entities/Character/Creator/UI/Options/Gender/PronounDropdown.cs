using Character.Creator;

public class PronounDropdown : ReactiveDropdown<CharacterPronouns>
{
	private ICustomizationSelectedDataRepository _dataRepo;

	protected override void Awake()
	{
		_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		base.Awake();
	}

	protected override MenuSettingsDropdownOption[] GetAllOptions()
	{
		var pronouns = new MenuSettingsDropdownOption[] {
			new MenuSettingsDropdownOption("He/Him", CharacterPronouns.HeHim),
			new MenuSettingsDropdownOption("She/Her", CharacterPronouns.SheHer),
			new MenuSettingsDropdownOption("They/Them", CharacterPronouns.TheyThem),
			new MenuSettingsDropdownOption("Zhey/Zhem", CharacterPronouns.ZheyZhem),
			new MenuSettingsDropdownOption("Custom", CharacterPronouns.Custom)
		};
		return pronouns;
	}
	protected override CharacterPronouns Value
	{
		get => _dataRepo?.CustomizationData?.GenderData?.Pronouns?.Val ?? CharacterPronouns.TheyThem;
		set
		{
			var pronouns = _dataRepo?.CustomizationData?.GenderData?.Pronouns;
			if (pronouns == null) return;
			pronouns.Val = value;
		}
	}
}