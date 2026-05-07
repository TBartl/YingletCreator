using UnityEngine;

public class PlaySoundOnRoll : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	private AudioPlayer _audioPlayer;
	private IRollProvider _rollProvider;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		_rollProvider = this.GetComponentInParentSafe<IRollProvider>();

		_rollProvider.OnRolled += OnRolled;
	}

	private void OnDestroy()
	{
		if (_rollProvider != null)
		{
			_rollProvider.OnRolled -= OnRolled;
		}
	}

	private void OnRolled(ICharacterRoot root)
	{
		_audioPlayer.Play(_soundEffect, new() { Position = root.transform.position });
	}
}