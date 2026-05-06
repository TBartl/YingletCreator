namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class PromptContinueNode : SingleOutputNode // For now; later we'll want to support multiple outputs for different choices
	{
		public override void Run(IEncounterInstance encounterInstance)
		{
			// Don't continue - the UI will do it
		}

		public override bool Blocking => true;

		public void Continue(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
		}
	}
}
