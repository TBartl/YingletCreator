using System.Collections.Generic;

namespace Encounters.Runtime
{
	public interface IEncounterNode
	{
		void EditorSetConnections(IList<IEncounterNode> connections);

		void Run(IEncounterInstance encounterInstance);
	}
}