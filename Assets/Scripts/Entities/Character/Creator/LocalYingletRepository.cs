using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Creator
{
	/// <summary>
	/// The group the yinglet belongs to.
	/// This determines where it shows up on the main screen,
	/// and enables / disables certain behavior
	/// </summary>
	public enum LocalYingletGroup
	{
		Preset,
		Custom,
		// Autosave(?)
	}

	public interface ILocalYingletRepository
	{
		IEnumerable<CachedYingletReference> GetYinglets(LocalYingletGroup group);
		IEnumerable<CachedYingletReference> GetAllYinglets();
		void AddNewCustom(CachedYingletReference reference);

		/// <summary>
		/// Removes the given yinglet from the custom repository, returning the index it existed at
		/// </summary>
		int DeleteCustom(CachedYingletReference reference);
	}

	public class LocalYingletRepository : MonoBehaviour, ILocalYingletRepository
	{
		private Dictionary<LocalYingletGroup, ObservableList<CachedYingletReference>> _yinglets = new();

		public IEnumerable<CachedYingletReference> GetYinglets(LocalYingletGroup group)
		{
			return _yinglets[group];
		}

		public IEnumerable<CachedYingletReference> GetAllYinglets()
		{
			return _yinglets.Values.SelectMany(list => list);
		}

		private void Awake()
		{
			LoadAllYinglets();
		}

		void LoadAllYinglets()
		{
			var dataLoader = this.GetComponent<IStartupYingletDataLoader>();
			LoadGroupYinglets(LocalYingletGroup.Preset);
			LoadGroupYinglets(LocalYingletGroup.Custom);

			void LoadGroupYinglets(LocalYingletGroup group)
			{
				var paths = dataLoader.LoadInitialYingData(group).ToArray();
				var list = new ObservableList<CachedYingletReference>();
				foreach (var path in paths)
				{
					list.Add(path);
				}
				_yinglets[group] = list;
			}
		}

		public void AddNewCustom(CachedYingletReference reference)
		{
			_yinglets[LocalYingletGroup.Custom].Add(reference);
		}

		public int DeleteCustom(CachedYingletReference reference)
		{
			var list = _yinglets[LocalYingletGroup.Custom];
			int index = list.IndexOf(reference);
			if (index < 0)
			{
				return -1;
			}
			list.RemoveAt(index);
			return index;
		}
	}
}
