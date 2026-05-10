using Reactivity;


public class EyeExpressionsMutator_Sleeping : ReactiveBehaviour, IBaseEyeExpressionMutator, IInitializable
{
	private ICharacterRoundState _roundState;

	public void Initialize()
	{
		_roundState = this.GetCharacterRootComponent<ICharacterRoundState>();
	}

	public EyeExpression Mutate(EyeExpression input)
	{
		if (_roundState.IsAsleep.Val)
		{
			return EyeExpression.Closed;
		}
		return input;
	}
}
