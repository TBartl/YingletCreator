using UnityEngine;

internal class PlaySoundOnTollEnergy : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	private IAudioPlayer _audioPlayer;
	private ITollEnergyOnEnterRoom _toll;

	private void Awake()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_toll = this.GetComponentInParentSafe<ITollEnergyOnEnterRoom>();
		_toll.OnEnergyTollApplied += OnTolled;
	}

	private void OnDestroy()
	{
		_toll.OnEnergyTollApplied -= OnTolled;
	}

	void OnTolled(int _)
	{
		_audioPlayer.Play(_soundEffect, new AudioPlayOptions() { Position = transform.position });
	}
}
