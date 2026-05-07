using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class RollNode : ContextNode, IEditorNode
	{
		const string ROLL_STAT_PORT_NAME = "Stat";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<StatId>(ROLL_STAT_PORT_NAME)
				.Build();

			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}
		public IEncounterNode CreateRuntimeNode()
		{
			StatId rollStat = this.GetPortValue<StatId>(ROLL_STAT_PORT_NAME);
			return new Runtime.RollNode(rollStat);
		}
	}
}