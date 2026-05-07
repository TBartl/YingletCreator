using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class ChangeCharacterResourceNode : Node, IEditorNode
	{
		const string RESOURCE_PORT_NAME = "Resource";
		const string DELTA_PORT_NAME = "Delta";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddInputPort<CharacterResourceId>(RESOURCE_PORT_NAME)
				.Build();

			context.AddInputPort<int>(DELTA_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{

			CharacterResourceId resource = this.GetPortValue<CharacterResourceId>(RESOURCE_PORT_NAME);
			int delta = this.GetPortValue<int>(DELTA_PORT_NAME);
			return new Runtime.ChangeCharacterResourceNode(resource, delta);
		}
	}
}
