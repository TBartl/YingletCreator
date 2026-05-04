using Encounters.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Encounters.Editor
{
	[Serializable]
	public class PlaySoundNode : Node, IEditorNode
	{
		const string SOUND_EFFECT_PORT_NAME = "Sound Effect";
		const string PLAY_LOCATION_PORT_NAME = "Play Location";

		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();

			context.AddInputPort<SoundEffect>(SOUND_EFFECT_PORT_NAME)
				.Build();

			context.AddInputPort<EncounterPlaySoundLocation>(PLAY_LOCATION_PORT_NAME)
				.Build();

			context.AddOutputPort(EditorNodeUtils.EXECUTION_PORT_NAME)
				.WithDisplayName(string.Empty)
				.WithConnectorUI(PortConnectorUI.Arrowhead)
				.Build();
		}

		public IEncounterNode CreateRuntimeNode()
		{
			SoundEffect soundEffect = this.GetPortValue<SoundEffect>(SOUND_EFFECT_PORT_NAME);
			EncounterPlaySoundLocation playLocation = this.GetPortValue<EncounterPlaySoundLocation>(PLAY_LOCATION_PORT_NAME);
			return new Runtime.PlaySoundNode(soundEffect, playLocation);
		}
	}
}
