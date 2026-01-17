using Reactivity;
using UnityEngine;

public class ReflectUITiltSetting : ReactiveBehaviour
{
	private ISettingsManager _settingsManager;
	private Quaternion _initialRotation;

	private void Start()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		_initialRotation = this.transform.rotation;
		AddReflector(ReflectSetting);
	}

	private void ReflectSetting()
	{
		this.transform.rotation = _settingsManager.Settings.UITilt ? _initialRotation : Quaternion.identity;
	}
}
