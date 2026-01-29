using UnityEngine;

namespace Character.Creator
{
	public class CustomizationDataSaveViaKeyboard : MonoBehaviour
	{
		[SerializeField] private SoundEffect _soundEffect;
		private IInputRestrictor _inputRestrictor;
		private IAudioPlayer _audioPlayer;
		private ISelectedYingletDiskIO _diskIO;

		private void Awake()
		{
			_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
			_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
			_diskIO = this.GetComponent<ISelectedYingletDiskIO>();
		}

		void Update()
		{
			if (!_inputRestrictor.InputAllowed) return; // Input not allowed
														// Manual saving too
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
			{
				bool succes = _diskIO.SaveSelected();
				if (succes)
				{
					_audioPlayer.Play(_soundEffect);
				}
			}
		}

		// Not even thinking about auto save yet lul
		// Maybe I'll add it in as an option but there's just too much risk of accidental overriding
	}
}
