namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class PromptChoiceNode : SingleOutputNode // For now; later we'll want to support multiple outputs for different choices
	{
		public override void Run(IEncounterInstance encounterInstance)
		{
			//encounterInstance.ProgressToNode(_next);
		}
	}
}
