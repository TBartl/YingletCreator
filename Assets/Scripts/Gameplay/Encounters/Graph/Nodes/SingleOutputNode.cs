using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public abstract class SingleOutputNode : IEncounterNode
	{
		[SerializeReference]
		protected IEncounterNode _next;


		public void EditorSetConnections(IList<IEncounterNode> connections)
		{
			if (connections.Count > 1)
			{
				throw new System.ArgumentException($"{nameof(SingleOutputNode)} can only have one connection, but {connections.Count} were provided.");
			}
			_next = connections.FirstOrDefault();
		}

		public virtual bool Blocking => false;

		public abstract void Run(IEncounterInstance encounterInstance);

	}
}
