using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class StartNode : Node, IEditorNode
	{
		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			return new Runtime.StartNode();
		}
	}
}