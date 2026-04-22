using Character.Creator;
using Character.Creator.UI;
using Reactivity;
using System.Linq;

public interface IExpeditionPlanningMemberReference : IPortraitReference
{
	bool IsNextForAdd { get; }
	ulong ClientId { get; }
}

public class ExpeditionPlanningMemberReference : ReactiveBehaviour, IExpeditionPlanningMemberReference, IClassReference
{
	private IExpeditionPlanningManager _planningManager;
	private int _siblingIndex;
	Computed<bool> _isNext;
	Computed<CachedYingletReference> _reference;

	public bool IsNextForAdd => _isNext.Val;
	public CachedYingletReference Reference => _reference.Val;

	public ulong ClientId => 0;

	public ClassId Class { get; private set; }

	void Awake()
	{
		_planningManager = Singletons.GetSingleton<IExpeditionPlanningManager>();

		_siblingIndex = transform.GetSiblingIndex();
		_isNext = CreateComputed(ComputeIsNext);
		_reference = CreateComputed(ComputeReference);

		// temp code
		Class = Singletons.GetSingleton<ICompositeResourceLoader>().LoadClasses().OrderBy(i => i.OrderIndex).ToArray()[_siblingIndex];
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
