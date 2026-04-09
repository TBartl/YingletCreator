using System;
using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class CharacterCreatorToggleIdButton : MonoBehaviour, IUserToggleEvents
	{
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorUndoManager _undoManager;
		private ICharacterCreatorToggleIdReference _reference;
		private Button _button;

		public event Action<bool> UserToggled;

		void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_reference = this.GetComponent<ICharacterCreatorToggleIdReference>();
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(Button_OnClick);
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(Button_OnClick);
		}


		private void Button_OnClick()
		{
			_undoManager.RecordState($"Toggle \"{_reference.ToggleId.DisplayName}\"");
			var from = _dataRepo.GetToggle(_reference.ToggleId);
			_dataRepo.FlipToggle(_reference.ToggleId);
			var to = _dataRepo.GetToggle(_reference.ToggleId);
			if (from != to)
			{
				UserToggled?.Invoke(to);
			}
		}
	}
}
