using Character.Creator;
using Character.Creator.UI;
using Reactivity;
using System.Linq;

public interface IExpeditionPlanningMemberReference : IPortraitReference
{
	bool IsNextForAdd { get; }
}

public class ExpeditionPlanningMemberReference : ReactiveBehaviour, IExpeditionPlanningMemberReference
{
	private IExpeditionPlanningManager _planningManager;
	private int _siblingIndex;
	Computed<bool> _isNext;
	Computed<CachedYingletReference> _reference;

	public bool IsNextForAdd => _isNext.Val;
	public CachedYingletReference Reference => _reference.Val;

	void Awake()
	{
		_planningManager = Singletons.GetSingleton<IExpeditionPlanningManager>();

		_siblingIndex = transform.GetSiblingIndex();
		_isNext = CreateComputed(ComputeIsNext);
		_reference = CreateComputed(ComputeReference);
	}

	private CachedYingletReference ComputeReference()
	{
		var party = _planningManager.CurrentParty.ToList();

		if (_siblingIndex < party.Count)
		{
			return party[_siblingIndex];
		}
		else
		{
			return null;
		}
	}

	private bool ComputeIsNext()
	{
		var party = _planningManager.CurrentParty.ToList();
		return _siblingIndex == party.Count;
	}
}
