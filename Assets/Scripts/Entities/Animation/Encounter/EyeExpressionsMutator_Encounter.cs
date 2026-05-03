using Encounters.Runtime;
using Reactivity;


public class EyeExpressionsMutator_Encounter : ReactiveBehaviour, IBaseEyeExpressionMutator, IInitializable
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

	public EyeExpression Mutate(EyeExpression input)
	{
		var expressions = _characterExpressions.Val;
		if (expressions == null) return input;
		return expressions.EyeExpression;
	}
}
