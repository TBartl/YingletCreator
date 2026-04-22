using Character.Creator;
using Reactivity;
using System.Linq;

public interface IExpeditionPlanningMemberReference : ICachedYingletReference
{
	ExpeditionPartyMember Reference { get; }
	bool IsNextForAdd { get; }
	ulong ClientId { get; }
}

public class ExpeditionPlanningMemberReference : ReactiveBehaviour, IExpeditionPlanningMemberReference, IClassReference
{
	private IExpeditionPlanningManager _planningManager;
	private int _siblingIndex;
	Computed<bool> _isNext;
	Computed<ExpeditionPartyMember> _reference;

	public bool IsNextForAdd => _isNext.Val;
	public ExpeditionPartyMember Reference => _reference.Val;

	public ulong ClientId => 0;

	public ClassId Class { get; private set; }

	public SerializableCustomizationData CachedData => _reference.Val?.CustomizationData;

	void Awake()
	{
		_planningManager = Singletons.GetSingleton<IExpeditionPlanningManager>();

		_siblingIndex = transform.GetSiblingIndex();
		_isNext = CreateComputed(ComputeIsNext);
		_reference = CreateComputed(ComputeReference);

		// temp code
		Class = Singletons.GetSingleton<ICompositeResourceLoader>().LoadClasses().OrderBy(i => i.OrderIndex).ToArray()[_siblingIndex];
	}

	private ExpeditionPartyMember ComputeReference()
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
