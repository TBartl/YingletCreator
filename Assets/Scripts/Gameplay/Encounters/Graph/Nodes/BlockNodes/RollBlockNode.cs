using UnityEngine;


namespace Encounters
{
	/// <summary>
	/// This is effectively cosmetic, but it's shown to the player to give them an idea how "good" the result is
	/// </summary>
	public enum RollClassification
	{
		CriticalFailure,
		Failure,
		Neutral,
		Success,
		CriticalSuccess
	}
}

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class RollBlockNode : SingleOutputNode
	{
		[field: SerializeField]
		public int MaxValueInclusive { get; private set; }

		[field: SerializeField]
		public RollClassification Classification { get; private set; }

		public RollBlockNode(int maxValue, RollClassification classification)
		{
			MaxValueInclusive = maxValue;
			Classification = classification;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
		}
	}
}
