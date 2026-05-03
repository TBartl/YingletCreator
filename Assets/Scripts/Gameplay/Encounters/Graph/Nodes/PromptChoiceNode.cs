using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class PromptChoiceNode : IEncounterNode
	{
		[field: SerializeField]
		public ChoiceBlockNode[] Choices { get; private set; }

		public void EditorSetConnections(IList<IEncounterNode> connections)
		{
			Choices = connections.Cast<ChoiceBlockNode>().ToArray();
		}

		public void Run(IEncounterInstance encounterInstance)
		{
			// UI will drive this and call back to our Continue function
		}

		public void Continue(IEncounterInstance encounterInstance, int choiceIndex)
		{
			encounterInstance.ProgressToNode(Choices[choiceIndex]);
		}
	}
}
