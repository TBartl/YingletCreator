using Reactivity;

public enum MouthExpression
{
	Grin,
	Frown,
	Muse,
	PLACEHOLDER, // The fourth slot is currently empty
	MAX
}

public enum MouthOpenAmount
{
	Closed,
	Ajar,
	Open,
	WideOpen,
	MAX
}

public interface IMouthExpressions
{
	MouthExpression Expression { get; }
	MouthOpenAmount OpenAmount { get; }
}

public interface IMouthExpressionsMutator
{

	public void Mutate(ref MouthExpression expression, ref MouthOpenAmount openAmount);
}

public class MouthExpressions : ReactiveBehaviour, IMouthExpressions
{
	private Observable<MouthExpression> _expression = new();
	private Observable<MouthOpenAmount> _openAmount = new();
	private IMouthExpressionsMutator[] _mutators;

	public MouthExpression Expression => _expression.Val;
	public MouthOpenAmount OpenAmount => _openAmount.Val;

	void Awake()
	{
		_mutators = this.GetComponentsSafe<IMouthExpressionsMutator>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var expression = MouthExpression.Grin;
		var openAmount = MouthOpenAmount.Closed;
		foreach (var mutator in _mutators)
		{
			mutator.Mutate(ref expression, ref openAmount);
		}
		_expression.Val = expression;
		_openAmount.Val = openAmount;
	}
}


