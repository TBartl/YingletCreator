using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Encounters.Editor
{
	[Serializable]
	public class CreateVfxNode : Node, IEditorNode
	{
		const string SPAWN_TARGET_PORT_NAME = "SpawnTarget";
		const string VFX_PREFAB_PORT_NAME = "VfxPrefab";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddInputPort<VfxSpawnTarget>(SPAWN_TARGET_PORT_NAME)
				.Build();

			context.AddInputPort<GameObject>(VFX_PREFAB_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			VfxSpawnTarget spawnTarget = this.GetPortValue<VfxSpawnTarget>(SPAWN_TARGET_PORT_NAME);
			GameObject vfxPrefab = this.GetPortValue<GameObject>(VFX_PREFAB_PORT_NAME);
			return new Runtime.CreateVfxNode(spawnTarget, vfxPrefab);
		}
	}
}