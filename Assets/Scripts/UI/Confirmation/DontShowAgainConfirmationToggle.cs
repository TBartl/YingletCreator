using Reactivity;
using UnityEngine.UI;

public class DontShowAgainConfirmationToggle : ReactiveBehaviour
{
	private IConfirmationManager _confirmationManager;
	private Toggle _toggle;

	private void Awake()
	{
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
		_toggle = this.GetComponentInChildren<Toggle>();
		_toggle.onValueChanged.AddListener(Toggle_OnValueChanged);
	}
	private void Start()
	{
		AddReflector(ReflectToggleValue);
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		_toggle.onValueChanged.RemoveListener(Toggle_OnValueChanged);
	}

	private void Toggle_OnValueChanged(bool arg0)
	{
		var current = _confirmationManager.Current.Val;
		if (current == null) return;
		current.DontShowAgain.Val = _toggle.isOn;
	}


	private void ReflectToggleValue()
	{
		var current = _confirmationManager.Current.Val;
		if (current == null) return;
		_toggle.SetIsOnWithoutNotify(current.DontShowAgain.Val);
	}
}
