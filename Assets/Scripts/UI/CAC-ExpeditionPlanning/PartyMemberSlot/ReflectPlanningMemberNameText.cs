using Reactivity;
using TMPro;

public class ReflectPlanningMemberNameText : ReactiveBehaviour
{
	private IExpeditionPlanningMemberReference _reference;
	private TMP_Text _text;

	void Start()
	{
		_reference = this.GetComponentInParent<IExpeditionPlanningMemberReference>();
		_text = this.GetComponent<TMP_Text>();
		AddReflector(ReflectText);
	}

	private void ReflectText()
	{
		var customizationData = _reference.CachedData;
		if (customizationData == null) return;
		_text.text = customizationData.Name;
	}
}
