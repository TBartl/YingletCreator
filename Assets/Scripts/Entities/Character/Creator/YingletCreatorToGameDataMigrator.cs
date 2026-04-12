using Character.Creator;
using System;
using System.IO;
using UnityEngine;

public class YingletCreatorToGameDataMigrator : MonoBehaviour
{
	private void Awake()
	{
		MigrateIfNeeded();
	}

	private void MigrateIfNeeded()
	{
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		var from = Path.Combine(documentsPath, "My Games", "Yinglet Creator", "CustomYings");
		var to = Singletons.GetSingleton<ICharacterCreatorFolderProvider>().CustomFolderRoot;

		if (!Directory.Exists(from))
		{
			// No old save folder found, skipping migration.
			return;
		}

		// Check if destination folder already has any files
		if (Directory.Exists(to) && Directory.GetFiles(to).Length > 0)
		{
			// Destination folder already has files, skipping migration to avoid overwriting.
			return;
		}

		// Copy all files from the old folder to the new folder
		foreach (var file in Directory.GetFiles(from))
		{
			var fileName = Path.GetFileName(file);
			var destFile = Path.Combine(to, fileName);
			File.Copy(file, destFile, overwrite: true);
		}
	}
}
