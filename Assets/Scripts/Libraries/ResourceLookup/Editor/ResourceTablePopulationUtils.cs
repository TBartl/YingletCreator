using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ResourceTablePopulationUtils
{
	public static void PopulateLookupTable(ModDefinition modDefinition)
	{
		bool isBuiltIn = modDefinition.IsBuiltInMod;
		string rootFolder = isBuiltIn ? "Assets/ScriptableObjects" : modDefinition.GetParentFolder();
		bool onlyIncludeItemsUnderFolder = !isBuiltIn;

		var presetYings = GatherYingPresets(rootFolder);
		var table = GenerateTable(rootFolder, onlyIncludeItemsUnderFolder);
		modDefinition.EditorSetPresetsAndTable(presetYings, table);
	}

	static string[] GatherYingPresets(string rootFolder)
	{
		var paths = Directory.GetFiles(rootFolder, "*.yingsave", SearchOption.AllDirectories);
		return paths.Select(path => File.ReadAllText(path)).ToArray();
	}

	static ResourceLookupTable GenerateTable(string rootFolder, bool onlyIncludeItemsUnderFolder)
	{
		// Find all ScriptableObject assets in project
		// I originally thought of just searching for the ones being used and working my way down but:
		// - Some assets are referenced by things in the UI
		// - Some are provided for modders (despite not being used itself)
		string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { rootFolder });
		var resources = new ResourcePair[guids.Length];

		Dictionary<string, Object> table = new Dictionary<string, Object>();

		foreach (var guid in guids)
		{
			LoadObjectAndAddRecursively(guid);
		}
		var lookupTable = new ResourceLookupTable()
		{
			Resources = table
				.Select(kvp => new ResourcePair(kvp.Key, kvp.Value))
				.ToArray()
		};

		// Debug.Log($"Populated lookup table with {table.Count} objects.");
		return lookupTable;

		void LoadObjectAndAddRecursively(string guid)
		{
			if (string.IsNullOrWhiteSpace(guid))
			{
				return;
			}
			if (guid == "00000000000000000000000000000000")
			{
				return; // If the field was set and then cleared, it will have a guid of all 0s
			}
			if (table.ContainsKey(guid))
			{
				return; // May have already added this if it was a reference from something else
			}

			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (onlyIncludeItemsUnderFolder && !IsDescendentOf(path, rootFolder))
			{
				return; // For mods, we don't want to add anything to the lookup table that's outside of our jurisdiction
			}

			Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
			table.Add(guid, obj);

			var fields = GetAssetReferenceFields(obj);
			foreach (var field in fields)
			{
				LoadObjectAndAddRecursively(field.AssetGUID);
			}
		}
	}
	public static IEnumerable<AssetReference> GetAssetReferenceFields(UnityEngine.Object target)
	{
		var results = new List<AssetReference>();
		var type = target.GetType();

		var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		while (type != null && type != typeof(UnityEngine.Object))
		{
			var fields = type.GetFields(flags | BindingFlags.DeclaredOnly);
			foreach (var field in fields)
			{
				if (!typeof(AssetReference).IsAssignableFrom(field.FieldType))
				{
					continue;
				}

				var instance = field.GetValue(target) as AssetReference;
				if (instance != null)
				{
					results.Add(instance);
				}
			}

			type = type.BaseType;
		}

		return results;
	}

	static bool IsDescendentOf(string path, string potentialRootPath)
	{
		var normalizedPath = Path.GetFullPath(path);
		var normalizedRootPath = Path.GetFullPath(potentialRootPath);
		return normalizedPath.StartsWith(normalizedRootPath);
	}
}
