using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class RollNode : ContextNode, IEditorNode
	{
		const string ROLL_TYPE_PORT_NAME = "Type";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<RollType>(ROLL_TYPE_PORT_NAME)
				.Build();

			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}
		public IEncounterNode CreateRuntimeNode()
		{
			RollType rollType = this.GetPortValue<RollType>(ROLL_TYPE_PORT_NAME);
			return new Runtime.RollNode(rollType);
		}
	}
}