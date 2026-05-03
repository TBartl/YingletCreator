using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[UseWithContext(typeof(PromptChoiceNode))]
	[Serializable]
	public class ChoiceBlockNode : BlockNode, IEditorNode
	{

		const string ENERGY_COST_PORT_NAME = "Energy Cost";
		const string TEXT_PORT_NAME = "Text";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<int>(ENERGY_COST_PORT_NAME)
				.Build();
			context.AddInputPort<string>(TEXT_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			string text = this.GetPortValue<string>(TEXT_PORT_NAME);
			int energyCost = this.GetPortValue<int>(ENERGY_COST_PORT_NAME);
			return new Runtime.ChoiceBlockNode(energyCost, text);
		}
	}
}
