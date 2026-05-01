using UnityEngine;

namespace Encounters.Runtime
{
	public sealed class EncounterGraph : ScriptableObject
	{
		[field: SerializeReference]
		public StartNode StartNode { get; private set; }


		public void EditorSetData(StartNode startNode)
		{
			StartNode = startNode;
		}
	}
}
