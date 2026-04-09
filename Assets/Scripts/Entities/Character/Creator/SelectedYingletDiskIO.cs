using System.Linq;
using UnityEngine;

namespace Character.Creator
{
	/// <summary>
	/// Performs IO operations for the yinglet using <see cref="IYingletDiskIO"/>,
	/// but only for the currently selected yinglet
	/// </summary>
	public interface ISelectedYingletDiskIO
	{
		/// <summary>
		/// Returns true if the save went through
		/// </summary>
		bool SaveSelected();
		void DuplicateSelected();
		void DeleteSelected();
	}

	internal class SelectedYingletDiskIO : MonoBehaviour, ISelectedYingletDiskIO
	{
		private IYingletDiskIO _yingletDiskIO;
		private ILocalYingletRepository _yingletRepository;
		private ICustomizationSelection _selectionReference;
		private ICustomizationSelectedDataRepository _selectionData;

		void Awake()
		{
			_yingletDiskIO = Singletons.GetSingleton<IYingletDiskIO>();
			_yingletRepository = Singletons.GetSingleton<ILocalYingletRepository>();
			_selectionReference = Singletons.GetSingleton<ICustomizationSelection>();
			_selectionData = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		}
		public bool SaveSelected()
		{
			if (_selectionReference.Selected == null) return false;
			var isPreset = _selectionReference.Selected.Group == LocalYingletGroup.Preset;
			if (isPreset) return false;

			var data = _selectionData.CustomizationData;

			var lastFilePath = _selectionReference.Selected.Path;
			var saveResults = _yingletDiskIO.Save(data, lastFilePath);


			// Update our own reference
			_selectionReference.Selected.CachedData = saveResults.SerializedData;
			_selectionReference.Selected.Path = saveResults.NewPath;

			_selectionReference.SelectionIsDirty = false;
			return true;
		}

		public void DuplicateSelected()
		{
			var data = _selectionData.CustomizationData;

			var newReference = _yingletDiskIO.Duplicate(data);

			_selectionReference.SetSelected(newReference);
		}


		public void DeleteSelected()
		{
			var index = _yingletDiskIO.Delete(_selectionReference.Selected);

			// Edge case: Undo of a delete action
			if (index == -1) return;

			// Select an adjacent item
			var customYinglets = _yingletRepository.GetYinglets(LocalYingletGroup.Custom);
			if (customYinglets.Any())
			{
				int elementId = Mathf.Max(0, (index - 1) % customYinglets.Count());
				var newSelection = customYinglets.ElementAt(elementId);
				_selectionReference.SetSelected(newSelection);
			}
			else
			{
				var newSelection = _yingletRepository.GetYinglets(LocalYingletGroup.Preset).First();
				_selectionReference.SetSelected(newSelection);
			}
		}

	}
}
