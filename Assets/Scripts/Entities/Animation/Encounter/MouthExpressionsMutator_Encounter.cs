using Encounters.Runtime;
using Reactivity;

public class MouthExpressionsMutator_Encounter : ReactiveBehaviour, IMouthExpressionsMutator, IInitializable
{
	private ICharacterEncounterReference _encounterReference;
	private Computed<FullCharacterExpressions> _characterExpressions;

	public void Initialize()
	{
		_encounterReference = this.GetCharacterRootComponent<ICharacterEncounterReference>();
		_characterExpressions = this.CreateComputed(ComputeCharacterExpressions);
	}

	private FullCharacterExpressions ComputeCharacterExpressions()
	{
		return _encounterReference.Encounter.Val?.Data?.CharacterExpressions;
	}

	public void Mutate(ref MouthExpression expression, ref MouthOpenAmount openAmount)
	{
		var expressions = _characterExpressions.Val;
		if (expressions == null)
		{
			return;
		}
		expression = expressions.MouthExpression;
		openAmount = expressions.MouthOpenAmount;
	}
}
