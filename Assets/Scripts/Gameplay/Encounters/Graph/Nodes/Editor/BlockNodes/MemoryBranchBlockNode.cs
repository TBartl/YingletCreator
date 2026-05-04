using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[UseWithContext(typeof(MemoryBranchNode))]
	[Serializable]
	public class MemoryBranchBlockNode : BlockNode, IEditorNode
	{
		const string VALUE_PORT_NAME = "Value";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<int>(VALUE_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			int value = this.GetPortValue<int>(VALUE_PORT_NAME);
			return new Runtime.MemoryBranchBlockNode(value);
		}
	}
}
