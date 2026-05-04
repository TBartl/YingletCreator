using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Encounters.Editor
{
	[Serializable]
	public class SetEncounterSpriteNode : Node, IEditorNode
	{
		const string SPRITE_PORT_NAME = "Sprite";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddInputPort<Sprite>(SPRITE_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}
		public IEncounterNode CreateRuntimeNode()
		{
			Sprite sprite = this.GetPortValue<Sprite>(SPRITE_PORT_NAME);
			return new Runtime.SetEncounterSpriteNode(sprite);
		}
	}
}