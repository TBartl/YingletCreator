using Reactivity;

public class MouthExpressionsMutator_Sleeping : ReactiveBehaviour, IMouthExpressionsMutator, IInitializable
{
	private ICharacterRoundState _roundState;

	public void Initialize()
	{
		_roundState = this.GetCharacterRootComponent<ICharacterRoundState>();
	}

	public void Mutate(ref MouthExpression expression, ref MouthOpenAmount openAmount)
	{
		if (_roundState.IsAsleep.Val)
		{
			openAmount = MouthOpenAmount.Closed;
		}
	}
}
