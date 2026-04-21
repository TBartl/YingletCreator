using Reactivity;
using TMPro;

public class ReflectPlanningMemberCountText : ReactiveBehaviour
{
	private IExpeditionPlanningManager _planningManager;
	private TMP_Text _text;

	void Start()
	{
		_planningManager = Singletons.GetSingleton<IExpeditionPlanningManager>();
		_text = this.GetComponent<TMP_Text>();
		AddReflector(ReflectText);
	}

	private void ReflectText()
	{
		var total = _planningManager.CurrentParty.Count;
		_text.text = $"{total} / {ExpeditionPlanningManager.MAX_CHARACTERS}";
	}
}
