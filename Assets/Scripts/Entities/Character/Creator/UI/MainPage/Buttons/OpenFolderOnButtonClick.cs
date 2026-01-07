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
			Process.Start("explorer.exe", folder);
		}
	}
}
