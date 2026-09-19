using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class PromptContinueButton : MonoBehaviour
{
	private IEncounterNodeReferenceUI _reference;
	private Button _button;

	void Start()
	{
		_reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_button = GetComponent<Button>();

		_button.onClick.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		if (_button == null) return;
		_button.onClick.RemoveListener(OnClick);
	}

	private void OnClick()
	{
		var data = _reference.Record.VisitData as PromptContinueNodeVisitData;
		data.SendMessage_Continue();
	}
}
