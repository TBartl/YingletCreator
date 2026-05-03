using Reactivity;

public enum EyeExpression
{
	Normal,
	Squint,
	Closed,
	ClosedHappy,
	Shocked,
	Angry,
	Sad,
	ClosedEnergy
}

public interface IEyeExpressionMutator
{
	/// <summary>
	/// Reflectively called, giving the implementation the opportunity to override the expression
	/// </summary>
	public EyeExpression Mutate(EyeExpression input);
}
public interface IBaseEyeExpressionMutator : IEyeExpressionMutator { }
public interface ICurrentEyeExpressionMutator : IEyeExpressionMutator { }

public interface IEyeExpressions
{
	/// <summary>
	/// The base expression is the expression pre-blink
	/// This is exposed separately since it's used for things like controlling antennas
	/// </summary>
	public EyeExpression BaseExpression { get; }

	/// <summary>
	/// The current expression is what's currently shown
	/// If the ying is blinking, this will reflect that even if the default is open
	/// </summary>
	public EyeExpression CurrentExpression { get; }
}

public class EyeExpressions : ReactiveBehaviour, IEyeExpressions
{

	private IBaseEyeExpressionMutator[] _baseMutators;
	private ICurrentEyeExpressionMutator[] _currentMutators;
	private Observable<EyeExpression> _baseExpression = new();
	private Observable<EyeExpression> _currentExpression = new();

	public EyeExpression BaseExpression => _baseExpression.Val;
	public EyeExpression CurrentExpression => _currentExpression.Val;

	void Awake()
	{
		_baseMutators = this.GetComponentsInChildrenSafe<IBaseEyeExpressionMutator>();
		_currentMutators = this.GetComponentsInChildrenSafe<ICurrentEyeExpressionMutator>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var expression = EyeExpression.Normal;
		// First pass, get things that qualify as a 
		foreach (var mutator in _baseMutators)
		{
			expression = mutator.Mutate(expression);
		}
		_baseExpression.Val = expression;
		foreach (var mutator in _currentMutators)
		{
			expression = mutator.Mutate(expression);
		}
		_currentExpression.Val = expression;
	}
}
