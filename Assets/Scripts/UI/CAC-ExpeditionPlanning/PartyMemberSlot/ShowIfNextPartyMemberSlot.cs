using Reactivity;

public class ShowIfNextPartyMemberSlot : ReactiveBehaviour
{
	private IExpeditionPlanningMemberReference _reference;

	void Start()
	{
		_reference = this.GetComponentInParent<IExpeditionPlanningMemberReference>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		this.gameObject.SetActive(_reference.IsNextForAdd);
	}
}
