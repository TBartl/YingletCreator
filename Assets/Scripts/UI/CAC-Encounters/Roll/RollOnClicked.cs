using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class RollOnClicked : MonoBehaviour
{
	private IConfirmationManager _confirmationManager;
	private RollNodeVisitData _data;
	private Button _button;

	void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;

		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_data.SendMessage_Roll(RollState.Rolling);
	}
}
