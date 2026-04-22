using UnityEngine;
using UnityEngine.UI;

public class RemovePlanningMemberOnClick : MonoBehaviour
{
	private IExpeditionPlanningManager _planningManager;
	private Button _button;
	private IExpeditionPlanningMemberReference _reference;

	void Start()
	{
		_planningManager = Singletons.GetSingleton<IExpeditionPlanningManager>();
		_button = GetComponent<Button>();
		_reference = this.GetComponentInParent<IExpeditionPlanningMemberReference>();

		_button.onClick.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		if (_button == null) return;
		_button.onClick.RemoveListener(OnClick);
	}

	private void OnClick()
	{
		_planningManager.RemoveFromParty(_reference.Reference.Id);
	}
}
