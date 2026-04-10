using Reactivity;
using System.Linq;
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

	public class CustomizationSelection : MonoBehaviour, ICustomizationSelection
	{
		private ILocalYingletRepository _yingletRepository;

		private Observable<CachedYingletReference> _selected = new Observable<CachedYingletReference>();

		void Awake()
		{
			_yingletRepository = Singletons.GetSingleton<ILocalYingletRepository>();

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
}
