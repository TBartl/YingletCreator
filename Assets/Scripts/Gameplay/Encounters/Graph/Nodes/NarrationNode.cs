using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class NarrationNode : SingleOutputNode
	{
		[field: SerializeField]
		public string Text { get; private set; }

		public NarrationNode(string text)
		{
			Text = text;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
		}
	}
}