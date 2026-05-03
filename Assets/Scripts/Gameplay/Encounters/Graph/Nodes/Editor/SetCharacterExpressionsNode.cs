using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class SetCharacterExpressionsNode : Node, IEditorNode
	{
		const string EYE_EXPRESSION_PORT_NAME = "EyeExpression";
		const string MOUTH_EXPRESSION_PORT_NAME = "MouthExpression";
		const string MOUTH_OPEN_AMOUNT_PORT_NAME = "MouthOpenAmount";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddInputPort<EyeExpression>(EYE_EXPRESSION_PORT_NAME)
				.Build();

			context.AddInputPort<MouthExpression>(MOUTH_EXPRESSION_PORT_NAME)
				.Build();

			context.AddInputPort<MouthOpenAmount>(MOUTH_OPEN_AMOUNT_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			EyeExpression eyeExpression = this.GetPortValue<EyeExpression>(EYE_EXPRESSION_PORT_NAME);
			MouthExpression mouthExpression = this.GetPortValue<MouthExpression>(MOUTH_EXPRESSION_PORT_NAME);
			MouthOpenAmount mouthOpenAmount = this.GetPortValue<MouthOpenAmount>(MOUTH_OPEN_AMOUNT_PORT_NAME);
			return new Runtime.SetCharacterExpressionsNode(eyeExpression, mouthExpression, mouthOpenAmount);
		}
	}
}