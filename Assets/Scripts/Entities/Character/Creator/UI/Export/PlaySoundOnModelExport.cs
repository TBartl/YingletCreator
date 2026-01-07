using UnityEngine;

public class PlaySoundOnModelExport : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	private IAudioPlayer _audioPlayer;
	private ExportOnButtonClickBase _exportOnButtonClick;

	private void Awake()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_exportOnButtonClick = this.GetComponent<ExportOnButtonClickBase>();
		_exportOnButtonClick.OnExport += OnExport;
	}

	private void OnDestroy()
	{
		_exportOnButtonClick.OnExport -= OnExport;
	}

	void OnExport()
	{
		_audioPlayer.Play(_soundEffect);
	}
}
