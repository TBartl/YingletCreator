using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
#endif

public class DisableAudioIfSymLinked : MonoBehaviour
{
#if UNITY_EDITOR
	IEnumerator Start()
	{
		// Wait a frame so this overrides the settings
		yield return null;
		yield return null;
		if (IsSymlinkedProject())
		{
			var audioMixerProvider = Singletons.GetSingleton<IAudioMixerProvider>();
			audioMixerProvider.Mixer.SetFloat("SoundEffectsVolume", -80f);
			audioMixerProvider.Mixer.SetFloat("MusicVolume", -80f);
			Debug.LogWarning("AudioMixerProvider has been disabled because this project is symlinked. Audio features may not work correctly in a symlinked project.");
		}
	}

	private bool IsSymlinkedProject()
	{
		// Get the Assets folder path
		string assetsPath = Application.dataPath; // Assets folder full path

		// Get the parent directory (project root)
		string projectRoot = System.IO.Path.GetDirectoryName(assetsPath);

		// Get the folder name containing the project
		string parentFolderName = System.IO.Path.GetFileName(projectRoot);

		// Check if folder name contains "-Symlinked"
		return parentFolderName.Contains("-Symlinked");
	}
#endif
}
