using UnityEngine;

namespace Character.Creator
{
	public class CustomizationDataSaveViaKeyboard : MonoBehaviour
	{
		[SerializeField] private SoundEffect _soundEffect;
		private ICharacterCreatorTracker _characterCreatorTracker;
		private IAudioPlayer _audioPlayer;
		private ISelectedYingletDiskIO _diskIO;

		private void Awake()
		{
			_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
			_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
			_diskIO = Singletons.GetSingleton<ISelectedYingletDiskIO>();
		}

		void Update()
		{
			if (_characterCreatorTracker.IsInCharacterCreator.Val == false) return;

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
