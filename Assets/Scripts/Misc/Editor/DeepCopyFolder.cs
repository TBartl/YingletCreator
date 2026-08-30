using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Misc.Editor
{
	public class DeepCopyFolder
	{
		[MenuItem("Custom/Deep Copy Folder")]
		public static void DeepCopy()
		{
			string sourceFolder = GetSourceFolder();
			string sourceFolderName = Path.GetFileName(sourceFolder);

			string destinationFolder = GetDestinationFolder(sourceFolder, sourceFolderName);
			string destinationFolderName = Path.GetFileName(destinationFolder);

			var fileMapping = new Dictionary<string, string>();

			CopyFolderRecursive(
				sourceFolder,
				destinationFolder,
				sourceFolderName,
				destinationFolderName,
				fileMapping
			);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			var pathsWithGuids = GetPathsWithGuids(fileMapping);

			UpdateGuidReferences(pathsWithGuids, sourceFolderName, destinationFolderName);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		static string GetSourceFolder()
		{
			var selectedObjects = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets);

			if (selectedObjects.Length != 1) throw new InvalidOperationException("Exactly one folder must be selected.");

			var sourceFolder = AssetDatabase.GetAssetPath(selectedObjects[0]);

			if (!AssetDatabase.IsValidFolder(sourceFolder)) throw new InvalidOperationException("The selected asset must be a folder.");

			return sourceFolder;
		}

		static string GetDestinationFolder(string sourceFolder, string sourceFolderName)
		{
			string destinationFolder = EditorUtility.SaveFolderPanel(
				"Copy folder to",
				Path.GetDirectoryName(sourceFolder),
				sourceFolderName
			);

			if (string.IsNullOrEmpty(destinationFolder)) throw new InvalidOperationException("The destination folder must be specified.");

			// Convert the absolute filesystem path returned by SaveFolderPanel
			// into a Unity asset path.
			destinationFolder = FileUtil.GetProjectRelativePath(destinationFolder);

			if (string.IsNullOrEmpty(destinationFolder)) throw new InvalidOperationException("The destination must be inside the Unity project.");

			if (AssetDatabase.IsValidFolder(destinationFolder))
			{
				if (AssetDatabase.FindAssets("", new[] { destinationFolder }).Length > 0)
					throw new InvalidOperationException("The target folder must be empty.");
			}
			else
			{
				string parent = Path.GetDirectoryName(destinationFolder);
				string name = Path.GetFileName(destinationFolder);

				AssetDatabase.CreateFolder(parent, name);
			}

			return destinationFolder;
		}

		private static void CopyFolderRecursive(
			string sourcePath,
			string destinationPath,
			string originalFolderName,
			string destinationFolderName,
			Dictionary<string, string> fileMapping)
		{
			// Copy files using Unity's AssetDatabase.
			foreach (string sourceFile in AssetDatabase.GetAllAssetPaths())
			{
				if (!sourceFile.StartsWith(sourcePath + "/", StringComparison.Ordinal)) continue;

				// Only process files directly inside this directory.
				string relative = sourceFile.Substring(sourcePath.Length + 1);

				if (relative.Contains("/")) continue;

				string fileName = Path.GetFileName(sourceFile);

				string newFileName = fileName.Replace(
					originalFolderName,
					destinationFolderName
				);

				string destinationFile = destinationPath + "/" + newFileName;

				if (!AssetDatabase.CopyAsset(sourceFile, destinationFile))
				{
					Debug.LogError($"Failed to copy asset '{sourceFile}' to '{destinationFile}'.");
				}
				else
				{
					fileMapping[sourceFile] = destinationFile;
				}
			}

			// Find immediate subdirectories.
			string[] subfolders = AssetDatabase.GetSubFolders(sourcePath);

			foreach (string sourceSubfolder in subfolders)
			{
				string folderName = Path.GetFileName(sourceSubfolder);

				string newFolderName = folderName.Replace(
					originalFolderName,
					destinationFolderName
				);

				string destinationSubfolder =
					destinationPath + "/" + newFolderName;

				AssetDatabase.CreateFolder(
					destinationPath,
					newFolderName
				);

				CopyFolderRecursive(
					sourceSubfolder,
					destinationSubfolder,
					originalFolderName,
					destinationFolderName,
					fileMapping
				);
			}
		}

		private static List<PathsWithGuids> GetPathsWithGuids(Dictionary<string, string> fileMapping)
		{
			var pathsWithGuids = new List<PathsWithGuids>();
			foreach (var kvp in fileMapping)
			{
				pathsWithGuids.Add(new PathsWithGuids(kvp.Key, kvp.Value));
			}
			return pathsWithGuids;
		}

		private static void UpdateGuidReferences(List<PathsWithGuids> pathsWithGuids, string sourceName, string destinationName)
		{

			// Update all destination files with new GUID references
			foreach (var pathGuid in pathsWithGuids)
			{
				string destinationFilePath = pathGuid.DestinationPath;
				string fileExtension = Path.GetExtension(destinationFilePath).ToLower();

				// Only process YAML-based assets (.asset, .prefab, .unity, etc.)
				if (!IsYamlBasedAsset(fileExtension)) continue;
				try
				{
					string fullPath = Path.Combine(Directory.GetCurrentDirectory(), destinationFilePath);
					string content = File.ReadAllText(fullPath);
					bool modified = false;

					// Replace all source GUIDs with destination GUIDs
					foreach (var pathWithGuid in pathsWithGuids)
					{
						string sourceGuid = pathWithGuid.SourceGuid;
						string destGuid = pathWithGuid.DestinationGuid;

						if (content.Contains(sourceGuid))
						{
							content = content.Replace(sourceGuid, destGuid);
							modified = true;
						}
					}

					// Replace all source names with destination names
					if (content.Contains(sourceName))
					{
						content = content.Replace(sourceName, destinationName);
						modified = true;
					}

					if (modified)
					{
						File.WriteAllText(fullPath, content);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError($"Failed to update GUID references in '{destinationFilePath}': {ex.Message}");
				}
			}

			static bool IsYamlBasedAsset(string extension)
			{
				return extension == ".asset" || extension == ".prefab" || extension == ".unity" || extension == ".mat";
			}
		}

		class PathsWithGuids
		{
			public string SourcePath { get; }
			public string SourceGuid { get; }
			public string DestinationPath { get; }
			public string DestinationGuid { get; }
			public PathsWithGuids(string sourcePath, string destinationPath)
			{
				SourcePath = sourcePath;
				SourceGuid = AssetDatabase.AssetPathToGUID(sourcePath);
				DestinationPath = destinationPath;
				DestinationGuid = AssetDatabase.AssetPathToGUID(destinationPath);
			}
		}
	}

}