using Reactivity;
using TMPro;

namespace Character.Creator.UI
{
	public class NameTextField : ReactiveBehaviour
	{
		private ICustomizationSelectedDataRepository _dataRepository;
		private ICustomizationSelection _selection;
		private TMP_InputField _inputField;

		private void Awake()
		{
			_dataRepository = this.GetComponentInParent<ICustomizationSelectedDataRepository>();
			_selection = this.GetComponentInParent<ICustomizationSelection>();
			_inputField = this.GetComponent<TMP_InputField>();
			_inputField.onValueChanged.AddListener(InputField_OnValueChanged);
		}

		private void Start()
		{
			AddReflector(ReflectText);
			AddReflector(ReflectInteractable);
		}


		void ReflectText()
		{
			_inputField.text = _dataRepository.CustomizationData.Name.Val;
		}

		private void ReflectInteractable()
		{
			var selected = _selection.Selected;
			if (selected == null)
			{
				_inputField.interactable = false;
			}
			_inputField.interactable = _selection.Selected.Group == LocalYingletGroup.Custom;
		}


		private new void OnDestroy()
		{
			base.OnDestroy();
			_inputField.onValueChanged.RemoveListener(InputField_OnValueChanged);
		}

		private void InputField_OnValueChanged(string arg0)
		{
			_dataRepository.CustomizationData.Name.Val = arg0;
		}
	}
}