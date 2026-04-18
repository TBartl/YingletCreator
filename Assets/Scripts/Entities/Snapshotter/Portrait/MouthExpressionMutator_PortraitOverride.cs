using Character.Creator;
using Reactivity;

public class MouthExpressionMutator_PortraitOverride : ReactiveBehaviour, IMouthExpressionsMutator
{
	private ICustomizationDataRepository _dataRepo;
	Computed<int> _intValueComputed;
	Computed<MouthExpression> _defaultExpressionComputed;
	Computed<MouthOpenAmount> _defaultOpenAmountComputed;

	void Awake()
	{
		_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();
		_intValueComputed = CreateComputed(ComputeDefaultIntValue);
		_defaultExpressionComputed = CreateComputed(ComputeDefaultExpression);
		_defaultOpenAmountComputed = CreateComputed(ComputeDefaultOpenAmount);
	}

	private int ComputeDefaultIntValue()
	{
		return _dataRepo.CustomizationData.PortraitData.OverrideMouthExpression.Val;
	}

	public void Mutate(ref MouthExpression expression, ref MouthOpenAmount openAmount)
	{
		if (!_dataRepo.CustomizationData.PortraitData.UseOverrideExpressions.Val) return;

		expression = _defaultExpressionComputed.Val;
		openAmount = _defaultOpenAmountComputed.Val;
	}

	private MouthExpression ComputeDefaultExpression()
	{
		return MouthExpressionsMutator_Default.GetExpressionFromInt(_intValueComputed.Val);
	}

	private MouthOpenAmount ComputeDefaultOpenAmount()
	{
		return MouthExpressionsMutator_Default.GetOpenAmountFromInt(_intValueComputed.Val);
	}
}
