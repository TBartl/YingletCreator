namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class StartNode : SingleOutputNode
	{
		public StartNode()
		{
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
		}

	}
}