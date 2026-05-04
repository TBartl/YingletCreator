using UnityEngine;

namespace Encounters.Runtime
{
	public enum EncounterPlaySoundLocation
	{
		EncounterSource,
		Character
	}

	[System.Serializable]
	public sealed class PlaySoundNode : SingleOutputNode
	{
		[SerializeField] private SoundEffect _soundEffect;
		[SerializeField] private EncounterPlaySoundLocation _playLocation;

		public PlaySoundNode(SoundEffect soundEffect, EncounterPlaySoundLocation playLocation)
		{
			_soundEffect = soundEffect;
			_playLocation = playLocation;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
			PlaySound(encounterInstance);
		}

		void PlaySound(IEncounterInstance encounterInstance)
		{
			var audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
			var position = _playLocation switch
			{
				EncounterPlaySoundLocation.EncounterSource => encounterInstance.EncounterSource.transform.position,
				EncounterPlaySoundLocation.Character => encounterInstance.Character.transform.position,
				_ => throw new System.NotImplementedException($"Unknown {nameof(EncounterPlaySoundLocation)}: {_playLocation}")
			};
			audioPlayer.Play(_soundEffect, new AudioPlayOptions() { Position = position });
		}
	}
}