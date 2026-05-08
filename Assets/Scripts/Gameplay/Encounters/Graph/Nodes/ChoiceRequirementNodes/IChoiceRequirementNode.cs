using System.Text;

namespace Encounters.Runtime
{
	public interface IChoiceRequirementNode
	{
		/// <summary>
		/// Returns true if the choice can be selected
		/// </summary>
		bool RequirementsMet(IEncounterInstance encounter);

		/// <summary>
		/// If the requirement is met and selected, this will be called to apply any additional consequences of the selection
		/// i.e. an energy cost reducing the player's energy
		/// </summary>
		void Apply(IEncounterInstance encounter);

		/// <summary>
		/// Appends the text to be displayed in the choice between the brackets
		/// </summary>
		void AppendDisplayText(StringBuilder sb);
	}

}