

using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class ChoiceRequirement_CostsResource : Node, IChoiceRequirementNode
	{
		const string RESOURCE_PORT_NAME = "Resource";
		const string NUMBER_PORT_NAME = "Number";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(RESOURCE_PORT_NAME)
				.WithDataType<CharacterResourceId>()
				.Build();

			context.AddInputPort(NUMBER_PORT_NAME)
				.WithDataType<int>()
				.Build();

			context.AddOutputPort(EditorNodeUtils.CHOICE_REQUIREMENTS_PORT_NAME)
				.WithDisplayName("")
				.WithDataType<ChoiceBlockRequirementPort>()
				.Build();
		}

		public Runtime.IChoiceRequirementNode CreateRuntimeNode()
		{
			var resourcePort = this.GetPortValue<CharacterResourceId>(RESOURCE_PORT_NAME);
			var numberPort = this.GetPortValue<int>(NUMBER_PORT_NAME);
			return new Runtime.ChoiceRequirement_CostsResource(resourcePort, numberPort);
		}
	}
}
