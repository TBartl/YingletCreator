using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class SetMemoryNode : Node, IEditorNode
	{
		const string KEY_PORT_NAME = "Key";
		const string VALUE_PORT_NAME = "Value";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddInputPort<string>(KEY_PORT_NAME)
				.Build();

			context.AddInputPort<int>(VALUE_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			string key = this.GetPortValue<string>(KEY_PORT_NAME);
			int value = this.GetPortValue<int>(VALUE_PORT_NAME);
			return new Runtime.SetMemoryNode(key, value);
		}
	}
}
