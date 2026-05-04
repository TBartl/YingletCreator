using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class ShakeCameraNode : Node, IEditorNode
	{

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			return new Runtime.ShakeCameraNode();
		}
	}
}
