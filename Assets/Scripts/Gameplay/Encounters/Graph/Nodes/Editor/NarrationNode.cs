using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class NarrationNode : Node, IEditorNode
	{
		const string TEXT_PORT_NAME = "Text";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddInputPort<string>(TEXT_PORT_NAME)
				.AsTextArea()
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			string text = this.GetPortValue<string>(TEXT_PORT_NAME);
			return new Runtime.NarrationNode(text);
		}
	}
}