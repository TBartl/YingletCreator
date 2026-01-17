using Reactivity;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class UITiltCheckbox : ReactiveBehaviour
	{
		protected ISettingsManager _settingsManager;
		private Toggle _toggle;

		private void Awake()
		{
			_settingsManager = Singletons.GetSingleton<ISettingsManager>();
			_toggle = this.GetComponentInChildren<Toggle>();
			_toggle.onValueChanged.AddListener(Toggle_OnValueChanged);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			_toggle.onValueChanged.RemoveListener(Toggle_OnValueChanged);
		}

		private void Toggle_OnValueChanged(bool arg0)
		{
			_settingsManager.Settings.UITilt = arg0;
			_settingsManager.SaveChangesToDisk();
		}


		private void Start()
		{
			AddReflector(ReflectToggleValue);
		}

		private void ReflectToggleValue()
		{
			_toggle.SetIsOnWithoutNotify(_settingsManager.Settings.UITilt);
		}


	}
}