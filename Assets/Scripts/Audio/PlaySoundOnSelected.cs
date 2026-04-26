using UnityEngine;

public class PlaySoundOnSelected : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	private IAudioPlayer _audioPlayer;
	private ISelectable _selectable;

	private void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_selectable = this.GetComponentSafe<ISelectable>();
		_selectable.Selected.OnChanged += OnSelectedChanged;

	}

	private void OnDestroy()
	{
		if (_selectable != null)
		{
			_selectable.Selected.OnChanged -= OnSelectedChanged;
		}
	}

	private void OnSelectedChanged(bool from, bool to)
	{
		if (to)
		{
			_audioPlayer.Play(_soundEffect);
		}
	}
}
