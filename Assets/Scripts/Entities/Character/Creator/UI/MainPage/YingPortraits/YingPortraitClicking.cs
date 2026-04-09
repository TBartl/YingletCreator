using Reactivity;
using System;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class YingPortraitClicking : ReactiveBehaviour
	{
		private ICustomizationSelection _selection;
		private IYingPortraitReference _reference;
		private ICharacterCreatorUndoManager _undoManager;
		private IConfirmationManager _confirmationManager;
		private Button _button;

		public event Action OnSelected = delegate { };

		private void Awake()
		{
			_selection = Singletons.GetSingleton<ICustomizationSelection>();
			_reference = this.GetComponent<IYingPortraitReference>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(Button_OnClick);

		}
		private void Start()
		{
			AddReflector(ReflectInteractable);
		}

		private new void OnDestroy()
		{
			base.OnDestroy();
			_button.onClick.RemoveListener(Button_OnClick);
		}

		private void Button_OnClick()
		{
			if (_selection.SelectionIsDirty)
			{
				_confirmationManager.OpenConfirmation(new(
					"Are you sure you want to switch yinglets?\n\nUnsaved changes will be lost.",
					"Discard Changes",
					"change-yinglet-selection",
					SetSelected));
			}
			else
			{
				SetSelected();
			}


			void SetSelected()
			{
				_undoManager.RecordState($"Selected yinglet \"{_reference.Reference.CachedData.Name}\"");
				_selection.SetSelected(_reference.Reference);
				OnSelected();
			}
		}
		void ReflectInteractable()
		{
			_button.interactable = !_reference.Selected.Val;
		}
	}
}
