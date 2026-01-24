using Character.Creator.UI;
using UnityEngine;
using UnityEngine.UI;

public class ChangeClipboardSelectionOnClick : MonoBehaviour
{
	[SerializeField] ClipboardSelectionType _selectionType;

	private IClipboardSelection _clipboardSelection;
	private Button _button;

	void Start()
	{
		_clipboardSelection = this.GetComponentInParent<IClipboardSelection>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_clipboardSelection.SetSelection(_selectionType);
	}
}
