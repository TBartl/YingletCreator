namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class ChoiceNode : SingleOutputNode // For now; later we'll want to support multiple outputs for different choices
	{
		public override void Run(IEncounterInstance encounterInstance)
		{
			//encounterInstance.ProgressToNode(_next);
		}
	}
}
