using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class MemoryBranchNode : IEncounterNode
	{
		[SerializeField] string _key;
		[SerializeField] MemoryBranchBlockNode[] _branches;

		public MemoryBranchNode(string key)
		{
			_key = key;
		}

		public void EditorSetConnections(IList<IEncounterNode> connections)
		{
			_branches = connections.Cast<MemoryBranchBlockNode>().ToArray();
		}

		public bool Blocking => false;

		public void Run(IEncounterInstance encounterInstance)
		{
			var memoryVal = encounterInstance.Memory.Read(_key);
			var branch = _branches.FirstOrDefault(b => b.Value == memoryVal);
			if (branch == null)
			{
				Debug.LogWarning($"MemoryBranchNode with key {_key} has no branch for value {memoryVal}");
			}
			encounterInstance.ProgressToNode(branch); // Ok to be null
		}
	}
}
