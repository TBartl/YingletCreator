using System;
using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class PortraitIdToggleButton : MonoBehaviour, IUserToggleEvents
	{
		private ICharacterCreatorUndoManager _undoManager;
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorTogglePortraitIdReference _reference;
		private Button _button;

		public event Action<bool> UserToggled;

		void Awake()
		{
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_reference = this.GetComponent<ICharacterCreatorTogglePortraitIdReference>();
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(Button_OnClick);
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(Button_OnClick);
		}


		private void Button_OnClick()
		{
			if (_dataRepo.CustomizationData == null) return;


			var from = _dataRepo.CustomizationData.PortraitData.PortraitId.Val;
			var to = _reference.PortraitId;
			if (from != to)
			{
				_undoManager.RecordState($"Change portrait \"{to.DisplayName}\"");
				_dataRepo.CustomizationData.PortraitData.PortraitId.Val = to;
				UserToggled?.Invoke(to);
			}
		}
	}
}
