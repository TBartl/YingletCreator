using Reactivity;
using System.Collections.Generic;
using System.Linq;
using TMPro;


public abstract class ReactiveDropdown<T> : ReactiveBehaviour
{

	private TMP_Dropdown _dropdown;
	private MenuSettingsDropdownOption[] _options;

	protected virtual void Awake()
	{
		_dropdown = this.GetComponentInChildren<TMP_Dropdown>();
		_dropdown.onValueChanged.AddListener(Dropdown_OnValueChanged);

		_options = GetAllOptions();

		PopulateDropdownOptions();
	}

	private void Start()
	{
		AddReflector(ReflectDropdownValue);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		_dropdown.onValueChanged.RemoveListener(Dropdown_OnValueChanged);
	}

	protected abstract MenuSettingsDropdownOption[] GetAllOptions();
	protected abstract T Value { get; set; }

	private void PopulateDropdownOptions()
	{
		_dropdown.ClearOptions();

		foreach (var option in _options)
		{
			_dropdown.options.Add(new TMP_Dropdown.OptionData(option.DisplayName));
		}
	}

	private void ReflectDropdownValue()
	{
		_dropdown.SetValueWithoutNotify(GetDropdownIndexForValue(Value));
	}

	protected virtual void Dropdown_OnValueChanged(int index)
	{
		Value = GetValueForDropdownIndex(index);
	}

	private int GetDropdownIndexForValue(T value)
	{
		return _options
			.Select((option, index) => new { option, index })
			.First(x => EqualityComparer<T>.Default.Equals(x.option.Value, value))
			.index;
	}

	private T GetValueForDropdownIndex(int index)
	{
		return _options[index].Value;
	}

	protected sealed class MenuSettingsDropdownOption
	{
		public string DisplayName { get; }
		public T Value { get; }
		public MenuSettingsDropdownOption(string displayName, T value)
		{
			DisplayName = displayName;
			Value = value;
		}
	}
}
