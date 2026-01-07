using System.IO;
using UnityEngine;

namespace Character.Creator
{
	public interface ICharacterCreatorFolderProvider
	{
		string CustomFolderRoot { get; }
		string PhotoRoot { get; }
	}

	public class CharacterCreatorFolderProvider : MonoBehaviour, ICharacterCreatorFolderProvider
	{
		private ISaveFolderProvider _saveFolderProvider;

		private void Awake()
		{
			_saveFolderProvider = Singletons.GetSingleton<ISaveFolderProvider>();
			CustomFolderRoot = PathUtils.EnsureDirectoryExists(Path.Combine(_saveFolderProvider.GameRootFolderPath, "CustomYings"));
			PhotoRoot = PathUtils.EnsureDirectoryExists(Path.Combine(_saveFolderProvider.GameRootFolderPath, "Photos"));
		}

		public string CustomFolderRoot { get; private set; }
		public string PhotoRoot { get; private set; }
	}
}
