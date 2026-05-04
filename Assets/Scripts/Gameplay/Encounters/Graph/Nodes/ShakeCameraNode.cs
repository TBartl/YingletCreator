namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class ShakeCameraNode : SingleOutputNode
	{
		public ShakeCameraNode()
		{
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
			PlaySound(encounterInstance);
		}

		void PlaySound(IEncounterInstance encounterInstance)
		{
			var cameraShaker = Singletons.GetSingleton<ICameraShaker>();
			cameraShaker.Shake();
		}
	}
}