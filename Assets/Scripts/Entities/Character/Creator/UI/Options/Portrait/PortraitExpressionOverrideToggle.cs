using Reactivity;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class PortraitExpressionOverrideToggle : ReactiveBehaviour
	{
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorUndoManager _undoManager;
		private Toggle _toggle;

		private void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
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
			_undoManager.RecordState("Toggle Override Expressions");
			_dataRepo.CustomizationData.PortraitData.UseOverrideExpressions.Val = arg0;
		}

		private void Start()
		{
			AddReflector(ReflectToggleValue);
		}

		private void ReflectToggleValue()
		{
			_toggle.SetIsOnWithoutNotify(_dataRepo.CustomizationData.PortraitData.UseOverrideExpressions.Val);
		}
	}

}