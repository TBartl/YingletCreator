using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Character.Creator
{

	internal interface IStartupYingletDataLoader
	{
		IEnumerable<CachedYingletReference> LoadInitialYingData(LocalYingletGroup group);
	}
	internal class StartupYingletDataLoader : MonoBehaviour, IStartupYingletDataLoader
	{
		public IEnumerable<CachedYingletReference> LoadInitialYingData(LocalYingletGroup group)
		{
			var data = group switch
			{
				LocalYingletGroup.Custom => LoadCustomYingData(),
				LocalYingletGroup.Preset => LoadPresetYingData(),
				_ => throw new System.NotImplementedException()
			};
			var dataList = data
				.Where(reference => reference.CachedData != null) // In case we got a corrupt yingsave file
				.ToList();
			dataList.Sort((a, b) => DateTime.Compare(a.CachedData.CreationTime, b.CachedData.CreationTime));
			return dataList;
		}

		IEnumerable<CachedYingletReference> LoadCustomYingData()
		{
			var diskIO = this.GetComponent<IYingletDiskIO>();
			return diskIO.LoadInitialCustomYingData();
		}

		IEnumerable<CachedYingletReference> LoadPresetYingData()
		{
			var loadMethod = Singletons.GetSingleton<IResourceLoadMethodProvider>().LoadMethod;
			return loadMethod switch
			{
				CompositeResourceLoadMethod.EditorAssetLookup => LoadPresetsFromEditor(),
				CompositeResourceLoadMethod.SerializedTableLookup => LoadPresetsFromMods(),
				_ => throw new NotImplementedException()
			};
		}

		IEnumerable<CachedYingletReference> LoadPresetsFromEditor()
		{
#if UNITY_EDITOR
			var paths = Directory.GetFiles(Application.dataPath, "*.yingsave", SearchOption.AllDirectories);
			return paths
			.Where(path => !path.Contains("Snapshot")) // No snapshot ying
			.Select(path =>
			{
				string text = File.ReadAllText(path);
				var data = SerializableCustomizationData.FromJSON(text);
				if (data == null)
				{
					Debug.LogError($"Failed to read editor yinglet at path {path}");
				}
				return new CachedYingletReference(path, data, LocalYingletGroup.Preset);
			});
#endif

#pragma warning disable CS0162 // Unreachable code detected
			return Enumerable.Empty<CachedYingletReference>();
#pragma warning restore CS0162 // Unreachable code detected
		}

		IEnumerable<CachedYingletReference> LoadPresetsFromMods()
		{
			var modLoader = Singletons.GetSingleton<IModLoader>();
			List<CachedYingletReference> loaded = new();

			foreach (var mod in modLoader.AllMods)
			{
				var rawStrings = mod.PresetYings;
				foreach (var rawString in rawStrings)
				{
					var data = SerializableCustomizationData.FromJSON(rawString);
					if (data == null)
					{
						Debug.LogError($"Failed to read preset yinglet in mod {mod.name}");
					}
					loaded.Add(new CachedYingletReference("", data, LocalYingletGroup.Preset));

				}
			}
			return loaded;
		}
	}
}
