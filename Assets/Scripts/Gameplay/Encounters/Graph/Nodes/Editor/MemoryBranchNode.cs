using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class MemoryBranchNode : ContextNode, IEditorNode
	{
		const string KEY_PORT_NAME = "Key";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<string>(KEY_PORT_NAME)
				.Build();

			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}
		public IEncounterNode CreateRuntimeNode()
		{
			string key = this.GetPortValue<string>(KEY_PORT_NAME);
			return new Runtime.MemoryBranchNode(key);
		}
	}
}