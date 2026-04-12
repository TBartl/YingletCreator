

using Character.Creator;
using Reactivity;
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

		var playerIdentity = this.GetComponentInParent<IPlayerIdentity>();
		if (!playerIdentity.IsMine) return; // Don't set a selection for something that isn't ours

		// TODO: Would be nice to remember the last one

		// Try to select first preset, or first custom as a backup
		var initialSelection = _yingletRepository.GetYinglets(LocalYingletGroup.Preset).FirstOrDefault();
		if (initialSelection == null) initialSelection = _yingletRepository.GetYinglets(LocalYingletGroup.Custom).First();
		_selected.Val = initialSelection;
	}

	public IReadOnlyObservable<CachedYingletReference> Selected => _selected;

	public bool SelectionIsDirty { get; set; }

	public void SetSelected(CachedYingletReference reference)
	{
		_selected.Val = reference;
		SelectionIsDirty = false;
	}
}
