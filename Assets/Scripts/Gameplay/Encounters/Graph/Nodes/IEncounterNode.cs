using System.Collections.Generic;

namespace Encounters.Runtime
{
	public interface IEncounterNode
	{
		void EditorSetConnections(IList<IEncounterNode> connections);

		void Run(IEncounterInstance encounterInstance);

		/// <summary>
		/// If the node generally blocks progress until some user input
		/// (i.e. prompted for continue or a roll)
		/// This is primarily used by the UI to determine what should be most visible to the user
		/// </summary>
		bool Blocking { get; }
	}
}