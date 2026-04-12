

using Character.Creator;
using Reactivity;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Per-character implementation of CustomizationSelection
/// This is used to track the selection separately in case multiple characters are spawned
/// </summary>
internal class GameCharacterCustomizationSelection : MonoBehaviour, ICustomizationSelection
{
	private ILocalYingletRepository _yingletRepository;

	private Observable<CachedYingletReference> _selected = new Observable<CachedYingletReference>();

	void Awake()
	{
		_yingletRepository = Singletons.GetSingleton<ILocalYingletRepository>();

		var allCharacters = _yingletRepository.GetAllYinglets();

		var playerIdentity = this.GetComponentInParent<IPlayerIdentity>();
		if (!playerIdentity.IsMine)
		{
			// Just set it to anything; the server should overwrite this shortly
			_selected.Val = allCharacters.FirstOrDefault();
			return;
		}


		var settingsManager = Singletons.GetSingleton<ISettingsManager>();
		var lastSelectedCharacterPath = settingsManager.Settings.LastSelectedCharacterPath;
		if (!string.IsNullOrWhiteSpace(lastSelectedCharacterPath))
		{
			// Try to find the last selected character
			var lastSelected = allCharacters.FirstOrDefault(character => Path.GetFileNameWithoutExtension(character.Path) == lastSelectedCharacterPath);

			if (lastSelected != null)
			{
				_selected.Val = lastSelected;
				return;
			}
		}

		// Try to select first preset, or first custom as a backup
		_selected.Val = allCharacters.FirstOrDefault();
	}

	public IReadOnlyObservable<CachedYingletReference> Selected => _selected;

	public bool SelectionIsDirty { get; set; }

	public void SetSelected(CachedYingletReference reference)
	{
		_selected.Val = reference;
		SelectionIsDirty = false;
	}
}
