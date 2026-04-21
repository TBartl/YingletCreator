using Reactivity;
using TMPro;

public class ReflectPartyMemberSlotUsername : ReactiveBehaviour
{
	private IExpeditionPlanningMemberReference _reference;
	private TMP_Text _text;
	private IClientNameLookup _nameLookup;
	Computed<string> _name;

	void Start()
	{
		_nameLookup = Singletons.GetSingleton<IClientNameLookup>();
		_reference = this.GetComponentInParent<IExpeditionPlanningMemberReference>();
		_text = this.GetComponent<TMP_Text>();

		_name = CreateComputed(ComputeName);
		AddReflector(ReflectName);
	}

	private string ComputeName()
	{
		return _nameLookup.GetNameForClient(_reference.ClientId);
	}

	private void ReflectName()
	{
		_text.text = _name.Val;
	}
}
