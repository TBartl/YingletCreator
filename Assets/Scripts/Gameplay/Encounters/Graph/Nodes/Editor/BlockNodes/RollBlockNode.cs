using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[UseWithContext(typeof(RollNode))]
	[Serializable]
	public class RollBlockNode : BlockNode, IEditorNode
	{
		const string MAX_VALUE_INCLUSIVE_PORT_NAME = "Max Value (Inclusive)";
		const string ROLL_CLASSIFICATION_PORT_NAME = "Classification";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<int>(MAX_VALUE_INCLUSIVE_PORT_NAME)
				.Build();

			context.AddInputPort<RollClassification>(ROLL_CLASSIFICATION_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			int value = this.GetPortValue<int>(MAX_VALUE_INCLUSIVE_PORT_NAME);
			RollClassification rollClassification = this.GetPortValue<RollClassification>(ROLL_CLASSIFICATION_PORT_NAME);
			return new Runtime.RollBlockNode(value, rollClassification);
		}
	}
}
