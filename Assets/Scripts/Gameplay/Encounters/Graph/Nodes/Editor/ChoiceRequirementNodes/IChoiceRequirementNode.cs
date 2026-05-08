namespace Encounters.Editor
{
	/// <summary>
	/// Marker class to restrict the type of nodes to be connected to only those that provide requirements
	/// </summary>
	public class ChoiceBlockRequirementPort { }

	/// <summary>
	/// Defines the interface for output-only nodes that can be connected to a choice to provide requirements
	/// </summary>
	public interface IChoiceRequirementNode
	{
		Runtime.IChoiceRequirementNode CreateRuntimeNode();
	}
}
