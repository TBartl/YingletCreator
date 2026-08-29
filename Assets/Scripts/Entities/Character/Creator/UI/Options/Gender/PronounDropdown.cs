using Character.Creator;

public class PronounDropdown : ReactiveDropdown<CharacterPronouns>
{
	private ICustomizationSelectedDataRepository _dataRepo;
	private ICharacterCreatorUndoManager _undoManager;
	//private ScrollContentUpdater _scrollContentUpdater;

	protected override void Awake()
	{
		base.Awake();
		_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
		//_scrollContentUpdater = new ScrollContentUpdater(this.transform);
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
			_undoManager.RecordState("Changed pronouns");

			//_scrollContentUpdater.ApplyAndRestoreScrollPosition(() =>
			//{
			pronouns.Val = value;
			//});
		}
	}
}