
using Reactivity;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Character.Creator.UI
{
	public class CustomPronounTextEntry : ReactiveBehaviour
	{
		[SerializeField] int index;

		private ICustomizationSelectedDataRepository _dataRepository;
		private ICharacterCreatorUndoManager _undoManager;
		private TMP_InputField _inputField;

		private void Awake()
		{
			_dataRepository = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_inputField = this.GetComponent<TMP_InputField>();
			_inputField.onValueChanged.AddListener(InputField_OnValueChanged);
		}

		private void Start()
		{
			AddReflector(ReflectText);
		}

		ObservableList<string> CustomPronouns => _dataRepository.CustomizationData?.GenderData?.CustomPronouns;

		void ReflectText()
		{
			var customPronouns = CustomPronouns;
			if (customPronouns == null)
			{
				_inputField.SetTextWithoutNotify("");
				return;
			}

			var pronoun = customPronouns.ElementAtOrDefault(index);
			_inputField.SetTextWithoutNotify(pronoun);
		}

		private new void OnDestroy()
		{
			base.OnDestroy();
			_inputField.onValueChanged.RemoveListener(InputField_OnValueChanged);
		}

		private void InputField_OnValueChanged(string arg0)
		{
			var customPronouns = CustomPronouns;
			if (customPronouns == null) return;

			_undoManager.RecordState("Changed pronouns");

			bool hadToSetup = false;
			while (customPronouns.Count <= index)
			{
				customPronouns.Add("");
				hadToSetup = true;
			}
			customPronouns[index] = arg0;

			// IDK why I'm having to do this, but if I don't the caret resets
			if (hadToSetup)
			{
				_inputField.caretPosition = _inputField.text.Length;
			}
		}
	}
}