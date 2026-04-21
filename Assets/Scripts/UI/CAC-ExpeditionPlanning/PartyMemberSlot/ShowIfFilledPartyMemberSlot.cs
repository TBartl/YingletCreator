using Reactivity;
using UnityEngine;

public class ShowIfFilledPartyMemberSlot : ReactiveBehaviour
{
	[SerializeField] bool _inverse = false;

	private IExpeditionPlanningMemberReference _reference;

	void Start()
	{
		_reference = this.GetComponentInParent<IExpeditionPlanningMemberReference>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		if (_inverse)
		{
			this.gameObject.SetActive(_reference.Reference == null);
		}
		else
		{
			this.gameObject.SetActive(_reference.Reference != null);
		}
	}
}
