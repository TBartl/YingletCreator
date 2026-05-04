using UnityEngine;


namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class MemoryBranchBlockNode : SingleOutputNode
	{
		[field: SerializeField]
		public int Value { get; private set; }

		public MemoryBranchBlockNode(int value)
		{
			Value = value;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
		}
	}
}
