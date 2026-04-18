using Character.Creator;
using Character.Data;
using Reactivity;
using UnityEngine;


public class MouthExpressionsMutator_Default : ReactiveBehaviour, IMouthExpressionsMutator
{
	[SerializeField] AssetReferenceT<CharacterIntId> _intIdReference;

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
		return _dataRepo.GetInt(_intIdReference.LoadSync());
	}

	private MouthExpression ComputeDefaultExpression()
	{
		return GetExpressionFromInt(_intValueComputed.Val);
	}

	private MouthOpenAmount ComputeDefaultOpenAmount()
	{
		return GetOpenAmountFromInt(_intValueComputed.Val);
	}

	public void Mutate(ref MouthExpression expression, ref MouthOpenAmount openAmount)
	{
		expression = _defaultExpressionComputed.Val;
		openAmount = _defaultOpenAmountComputed.Val;
	}

	public const int Columns = 4;
	public static MouthExpression GetExpressionFromInt(int i) => (MouthExpression)(i / Columns);
	public static MouthOpenAmount GetOpenAmountFromInt(int i) => (MouthOpenAmount)(i % Columns);
}
