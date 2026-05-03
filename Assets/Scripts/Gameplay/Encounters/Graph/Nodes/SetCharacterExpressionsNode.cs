using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class FullCharacterExpressions
	{
		[field: SerializeField] public EyeExpression EyeExpression { get; private set; }
		[field: SerializeField] public MouthExpression MouthExpression { get; private set; }
		[field: SerializeField] public MouthOpenAmount MouthOpenAmount { get; private set; }
		public FullCharacterExpressions(EyeExpression eyeExpression, MouthExpression mouthExpression, MouthOpenAmount mouthOpenAmount)
		{
			EyeExpression = eyeExpression;
			MouthExpression = mouthExpression;
			MouthOpenAmount = mouthOpenAmount;
		}
	}

	[System.Serializable]
	public class SetCharacterExpressionsNode : SingleOutputNode
	{
		[SerializeField] private FullCharacterExpressions _expressions;

		public SetCharacterExpressionsNode(FullCharacterExpressions expressions)
		{
			_expressions = expressions;
		}

		public SetCharacterExpressionsNode(EyeExpression eyeExpression, MouthExpression mouthExpression, MouthOpenAmount mouthOpenAmount)
		{
			_expressions = new FullCharacterExpressions(eyeExpression, mouthExpression, mouthOpenAmount);
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.Data.CharacterExpressions = _expressions;
			encounterInstance.ProgressToNode(_next);
		}
	}
}