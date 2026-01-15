using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


namespace Character.Creator.UI
{
	public enum OpenFolderOnClickType
	{
		Custom,
		Export,
		Photos
	}

	public class OpenFolderOnButtonClick : MonoBehaviour
	{
		[SerializeField] private OpenFolderOnClickType _type;

		private Button _button;
		private ISaveFolderProvider _rootFolderProvider;
		private ICharacterCreatorFolderProvider _characterCreatorFolderProvider;

		private void Awake()
		{
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(Button_OnClick);

			_rootFolderProvider = Singletons.GetSingleton<ISaveFolderProvider>();
			_characterCreatorFolderProvider = this.GetComponentInParent<ICharacterCreatorFolderProvider>();
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(Button_OnClick);
		}

		private void Button_OnClick()
		{
			var folder = _type switch
			{
				OpenFolderOnClickType.Custom => _characterCreatorFolderProvider.CustomFolderRoot,
				OpenFolderOnClickType.Export => _rootFolderProvider.ExportsFolderPath,
				OpenFolderOnClickType.Photos => _characterCreatorFolderProvider.PhotoRoot,
				_ => _characterCreatorFolderProvider.CustomFolderRoot
			};
			OpenFolder(folder);
		}

		public static void OpenFolder(string folderPath)
		{
			// Ensure it is quoted.
			if (!folderPath.StartsWith("\""))
			{
				folderPath = "\"" + folderPath;
			}
			if (!folderPath.EndsWith("\""))
			{
				folderPath = folderPath + "\"";
			}
			// C# does not provide a cross-platform way to open folders, so use OS-specific commands.
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			Process.Start("explorer.exe", folderPath);
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
			Process.Start("xdg-open", folderPath);
#else // macOS and others.
			Process.Start("open", folderPath);
#endif
		}
	}
}
