using Encounters.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[UseWithContext(typeof(PromptChoiceNode))]
	[Serializable]
	public class ChoiceBlockNode : BlockNode, IEditorNode
	{
		const string TEXT_PORT_NAME = "Text";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<string>(TEXT_PORT_NAME)
				.Build();

			context.AddInputPort(EditorNodeUtils.CHOICE_REQUIREMENTS_PORT_NAME)
				.WithDataType<ChoiceBlockRequirementPort>()
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			string text = this.GetPortValue<string>(TEXT_PORT_NAME);
			var requirements = GetChoiceRequirements();

			return new Runtime.ChoiceBlockNode(text, requirements);
		}

		private Runtime.IChoiceRequirementNode[] GetChoiceRequirements()
		{
			var requirementsPort = this.GetInputPortByName(EditorNodeUtils.CHOICE_REQUIREMENTS_PORT_NAME);
			List<IPort> connectedPorts = new();
			requirementsPort.GetConnectedPorts(connectedPorts);
			return connectedPorts
				.Select(port => port.GetNode())
				.OfType<IChoiceRequirementNode>()
				.Select(node => node.CreateRuntimeNode())
				.ToArray();
		}
	}
}
