using UnityEngine;

public class PlaySoundOnOpenConfirmation : MonoBehaviour
{

	[SerializeField] private SoundEffect _soundEffect;
	private IAudioPlayer _audioPlayer;
	private IConfirmationManager _confirmationManager;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();

		_confirmationManager.Current.OnChanged += OnChanged;
	}

	private void OnDestroy()
	{
		_confirmationManager.Current.OnChanged -= OnChanged;
	}

	private void OnChanged(ConfirmationData data1, ConfirmationData data2)
	{
		if (data2 != null)
		{
			_audioPlayer.Play(_soundEffect);
		}
	}
}
