using Reactivity;
using UnityEngine;

namespace Character.Creator
{
	/// <summary>
	/// Returns observable data associated to the selected yinglet
	/// </summary>
	public interface ICustomizationSelection
	{
		public IReadOnlyObservable<CachedYingletReference> Selected { get; }

		public void SetSelected(CachedYingletReference reference);

		public bool SelectionIsDirty { get; set; }
	}

	/// <summary>
	/// Singleton implementation that just reflects the current yinglet as a convenient wrapper
	/// </summary>
	public class CustomizationSelection : ReactiveBehaviour, ICustomizationSelection
	{
		private IActiveCharacterProvider _activeCharacterProvider;
		Computed<ICustomizationSelection> _currentGameCharacterSelection;
		Computed<CachedYingletReference> _selected;

		private void Awake()
		{
			_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();

			_currentGameCharacterSelection = CreateComputed(() =>
			{
				var currentCharacter = _activeCharacterProvider.ActiveCharacter.Val;
				if (currentCharacter == null) return null;
				return currentCharacter.GetComponentInChildrenSafe<ICustomizationSelection>();
			});
			_selected = CreateComputed(() =>
			{
				var currentSelection = _currentGameCharacterSelection.Val;
				if (currentSelection == null) return null;
				var selected = currentSelection.Selected.Val;
				if (selected == null) return null;
				return selected;
			});
		}

		public IReadOnlyObservable<CachedYingletReference> Selected => _selected;

		public bool SelectionIsDirty
		{
			get
			{
				var currentSelection = _currentGameCharacterSelection.Val;
				if (currentSelection == null) return false;
				return currentSelection.SelectionIsDirty;
			}
			set
			{
				var currentSelection = _currentGameCharacterSelection.Val;
				if (currentSelection == null) return;
				currentSelection.SelectionIsDirty = value;
			}
		}

		public void SetSelected(CachedYingletReference reference)
		{
			var currentSelection = _currentGameCharacterSelection.Val;
			if (currentSelection == null)
			{
				Debug.LogWarning("No current character selection found, cannot set selected yinglet");
				return;
			}
			currentSelection.SetSelected(reference);
		}
	}
}
