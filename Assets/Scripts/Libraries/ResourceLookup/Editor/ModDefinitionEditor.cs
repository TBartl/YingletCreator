using Snapshotter;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModDefinition))]
public class ModDefinitionEditor : Editor
{
	SerializedProperty _titleProp;
	SerializedProperty _shortDescriptionProp;
	SerializedProperty _authorProp;
	SerializedProperty _iconProp;
	private bool _showAdvancedSettings;
	private void EnsureProperties()
	{
		if (_titleProp != null)
			return;

		_titleProp = serializedObject.FindProperty("_title");
		_shortDescriptionProp = serializedObject.FindProperty("_shortDescription");
		_authorProp = serializedObject.FindProperty("_author");
		_iconProp = serializedObject.FindProperty("_icon");
	}

	public override void OnInspectorGUI()
	{
		EnsureProperties();

		serializedObject.Update();
		var modDefinition = (ModDefinition)target;

		EditorGUILayout.LabelField("These will display on the mods page in-app:");
		EditorGUILayout.PropertyField(_titleProp);
		EditorGUILayout.PropertyField(_shortDescriptionProp);
		EditorGUILayout.PropertyField(_authorProp);
		EditorGUILayout.PropertyField(_iconProp, new GUIContent("Icon (256x256 for Steam)"));

		DrawHorizontalLine(Color.gray);
		EditorGUILayout.LabelField("Click the following button to generate toggle and pose icons");
		EditorGUILayout.LabelField("Note: This can only be done while the game is running in-editor");

		if (GUILayout.Button("Generate Icons"))
		{
			if (EditorApplication.isPlaying)
			{
				SnapshotToSpriteSheetUtils.GenerateToggleIcons(modDefinition);
				SnapshotToSpriteSheetUtils.GeneratePoseIcons(modDefinition);
				SnapshotToSpriteSheetUtils.GeneratePortraitIcons(modDefinition);
				SnapshotToSpriteSheetUtils.UpdateIconsInScene();
				EditorUtility.DisplayDialog("Generate Icons", $"Icons generated!", "OK");
			}
			else
			{
				EditorUtility.DisplayDialog("Generate Icons", $"The game must be running to generate icons; hit the play button above.", "OK");
			}
		}

		DrawHorizontalLine(Color.gray);

		if (modDefinition.IsBuiltInMod)
		{
			EditorGUILayout.LabelField("The default mod can not be built; it is automatically included");

			if (GUILayout.Button("Regenerate Lookup Table"))
			{
				ResourceTablePopulationUtils.PopulateLookupTable(modDefinition);
			}
		}

		else
		{
			EditorGUILayout.LabelField("Click one of the following buttons build the mod. The mod will include all of the following items nested in this folder:");
			EditorGUILayout.LabelField(" • Presets (.yingsave)");
			EditorGUILayout.LabelField(" • Toggles (CharacterToggleId)");
			EditorGUILayout.LabelField(" • Poses (PoseID)");
			EditorGUILayout.LabelField("Supporting textures, models, and ScriptableObject metadata will also be bundled.");

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Build .yingmod for\nfile-based distribution"))
			{
				BuildAsync(modDefinition, false);
			}
			if (GUILayout.Button("Build and publish\nto the Steam workshop"))
			{
				BuildAsync(modDefinition, true); // Fire and forget
			}
			EditorGUILayout.EndHorizontal();
		}

		_showAdvancedSettings = EditorGUILayout.Foldout(_showAdvancedSettings, "Advanced Settings", true);
		if (_showAdvancedSettings)
		{
			if (!modDefinition.IsBuiltInMod)
			{
				EditorGUI.indentLevel++;
				if (GUILayout.Button("Clear steam workshop item ID\nThis will cause the publish button to upload as a new item instead of updating\nUseful if you deleted the item on the workshop"))
				{
					modDefinition.SteamWorkshopId = 0;
					EditorUtility.SetDirty(modDefinition);
					AssetDatabase.SaveAssets();
				}
				EditorGUI.indentLevel--;
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

	private static void DrawHorizontalLine(Color color, float thickness = 1f, float padding = 6f)
	{
		EditorGUILayout.Space();

		Rect rect = EditorGUILayout.GetControlRect(false, thickness + padding);
		rect.height = thickness;
		rect.y += padding / 2f;
		EditorGUI.DrawRect(rect, color);
		EditorGUILayout.Space();
	}

	static async void BuildAsync(ModDefinition modDefinition, bool steamWorkshop)
	{
		TemporaryFolder tempOutputFolder = steamWorkshop ? new TemporaryFolder() : null;
		try
		{
			string modAssetPath = AssetDatabase.GetAssetPath(modDefinition);
			string modRelativeFolder = Path.GetDirectoryName(modAssetPath);
			string bundleFileName = Path.GetFileNameWithoutExtension(modAssetPath) + ModDefinition.ModExtension;
			string outputFolder = steamWorkshop ? tempOutputFolder.Path : GetBuildModsOutputFolder();

			EditorUtility.DisplayProgressBar($"Building Mod - {bundleFileName}", "Assigning assets to bundle...", 0.1f);
			var assetPaths = GetAssetPathsInFolder(modRelativeFolder);
			AssignAssetBundleToAssets(assetPaths, modDefinition.UniqueAssetID);

			EditorUtility.DisplayProgressBar($"Building Mod - {bundleFileName}", $"Creating asset lookup table...", 0.2f);
			ResourceTablePopulationUtils.PopulateLookupTable(modDefinition);

			EditorUtility.DisplayProgressBar($"Building Mod - {bundleFileName}", "Saving updated properties on ModDefinition...", 0.4f);
			EditorUtility.SetDirty(modDefinition);
			AssetDatabase.SaveAssets();

			EditorUtility.DisplayProgressBar($"Building Mod - {bundleFileName}", "Building bundle...", 0.6f);
			BuildBundle(outputFolder, bundleFileName, assetPaths);

			EditorUtility.DisplayProgressBar($"Building Mod - {bundleFileName}", "Cleaning up...", 0.8f);
			DeleteAllManifestFiles(outputFolder);

			if (steamWorkshop)
			{
				EditorUtility.DisplayProgressBar($"Building Mod - {bundleFileName}", "Publishing to steam...", 0.8f);
				await SteamWorkshopUploading.UploadModAsync(modDefinition, outputFolder);
			}

			EditorUtility.ClearProgressBar();
			if (steamWorkshop)
			{
				EditorUtility.DisplayDialog($"Mod Built - {bundleFileName}", $"Mod contents published to steam; use browser to finish setting up page and make visible", "OK");
			}
			else
			{
				EditorUtility.RevealInFinder(Path.Combine(outputFolder, bundleFileName));
				EditorUtility.DisplayDialog($"Mod Built - {bundleFileName}", $"Mod contents built to: {outputFolder}", "OK");
			}
		}
		catch (Exception ex)
		{
			EditorUtility.ClearProgressBar();
			Debug.LogException(ex);
			EditorUtility.DisplayDialog("Bundle Content", $"An error occurred while bundling: {ex.Message}\nSee Console for details.", "OK");
		}
		finally
		{
			tempOutputFolder?.Dispose();
		}
	}

	static string[] GetAssetPathsInFolder(string relativeFolder)
	{

		var assetGuids = AssetDatabase.FindAssets("", new[] { relativeFolder });
		return assetGuids
			.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			.Where(path => !path.EndsWith(".yingsave"))
			.ToArray();
	}

	static void AssignAssetBundleToAssets(string[] assetPaths, string bundleName)
	{
		foreach (var assetPath in assetPaths)
		{
			var importer = AssetImporter.GetAtPath(assetPath);
			if (importer != null && importer.assetBundleName != bundleName)
			{
				importer.assetBundleName = bundleName;
				importer.SaveAndReimport();
			}
		}
		AssetDatabase.RemoveUnusedAssetBundleNames(); // clean up any stale bundle names
	}

	static string GetBuildModsOutputFolder()
	{
		var projectRoot = Directory.GetParent(Application.dataPath).FullName;
		var outputFolder = Path.Combine(projectRoot, "Builds", "Mods");
		return outputFolder;
	}

	static void BuildBundle(string outputFolder, string bundleFileName, string[] assetPaths)
	{
		PathUtils.EnsureDirectoryExists(outputFolder);
		var build = new AssetBundleBuild()
		{
			assetBundleName = bundleFileName,
			assetNames = assetPaths
		};
		var buildParams = new BuildAssetBundlesParameters()
		{
			outputPath = outputFolder,
			bundleDefinitions = new[] { build },
			targetPlatform = EditorUserBuildSettings.activeBuildTarget
		};
		var manifest = BuildPipeline.BuildAssetBundles(buildParams);
	}

	static void DeleteAllManifestFiles(string outputFolder)
	{
		// Delete all .manifest files (recursive)
		foreach (string file in Directory.GetFiles(outputFolder))
		{
			if (!file.EndsWith(ModDefinition.ModExtension))
			{
				File.Delete(file);
			}
		}
	}
}