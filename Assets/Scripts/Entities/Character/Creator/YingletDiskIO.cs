using Reactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Character.Creator
{
	public sealed class CachedYingletReference
	{
		Observable<SerializableCustomizationData> _cachedData;

		public CachedYingletReference(string path, SerializableCustomizationData cachedData, LocalYingletGroup group)
		{
			Path = path;
			_cachedData = new Observable<SerializableCustomizationData>(cachedData);
			Group = group;
		}

		public string Path { get; set; }
		public SerializableCustomizationData CachedData
		{
			get
			{
				return _cachedData.Val;
			}
			set
			{
				_cachedData.Val = value;
			}
		}

		public LocalYingletGroup Group { get; }
	}

	/// <summary>
	/// Provides mechanisms for reading / writing character customization data from the disk
	/// </summary>
	public interface IYingletDiskIO
	{
		YingletDiskSaveResults Save(ObservableCustomizationData observableData, string lastFilePath);

		CachedYingletReference Duplicate(ObservableCustomizationData observableData);

		/// <summary>
		/// Returns the index of what was deleted
		/// </summary>
		int Delete(CachedYingletReference reference);

		IEnumerable<CachedYingletReference> LoadInitialCustomYingData();

		/// <summary>
		/// Event fired when a yinglet is saved to disk
		/// Text is the name of the file
		/// </summary>
		event Action<string> OnSaved;

		/// <summary>
		/// Event fired when a yinglet is deleted
		/// Text is the name of the file
		/// </summary>
		event Action<string> OnDeleted;
	}

	public sealed class YingletDiskSaveResults
	{
		public SerializableCustomizationData SerializedData { get; }
		public string NewPath { get; }
		public YingletDiskSaveResults(SerializableCustomizationData serializedData, string newPath)
		{
			SerializedData = serializedData;
			NewPath = newPath;
		}
	}


	public class YingletDiskIO : MonoBehaviour, IYingletDiskIO
	{
		const string EXTENSION = ".yingsave";
		const string DUPLICATE_SUFFIX = " - Copy";

		private ICharacterCreatorFolderProvider _locationProvider;
		private ILocalYingletRepository _yingletRepository;

		public event Action<string> OnSaved = delegate { };
		public event Action<string> OnDeleted = delegate { }; // Added event implementation

		void Awake()
		{
			_locationProvider = this.GetComponent<ICharacterCreatorFolderProvider>();
			_yingletRepository = this.GetComponent<ILocalYingletRepository>();
		}

		public YingletDiskSaveResults Save(ObservableCustomizationData observableData, string lastFilePath)
		{
			// Serialize the data
			var serializedData = new SerializableCustomizationData(observableData);

			// Write it to disk
			string rootFolder = _locationProvider.CustomFolderRoot;
			string newYingletName = observableData.Name.Val;
			var newFilePath = GetUniqueAlphanumericFilePath(newYingletName, lastFilePath, rootFolder);
			WriteToDisk(newFilePath, serializedData);

			// Clean up the old path (if applicable)
			bool pathIsTheSame = newFilePath == lastFilePath;
			if (!pathIsTheSame)
			{
				File.Delete(lastFilePath);
			}

			OnSaved(Path.GetFileName(newFilePath));

			return new YingletDiskSaveResults(serializedData, newFilePath);
		}

		public CachedYingletReference Duplicate(ObservableCustomizationData observableData)
		{
			// Serialize the data
			bool debugButtonsHeld = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl);
			if (!debugButtonsHeld)
			{
				observableData.Name.Val += DUPLICATE_SUFFIX;
			}
			var serializedData = new SerializableCustomizationData(observableData);
			serializedData.CreationTime = debugButtonsHeld ? observableData.CreationTime : DateTime.Now;

			// Write it to disk
			string rootFolder = _locationProvider.CustomFolderRoot;
			string newYingletName = observableData.Name.Val;
			var newFilePath = GetUniqueAlphanumericFilePath(newYingletName, null, rootFolder);
			WriteToDisk(newFilePath, serializedData);

			// Create a new reference and select it
			var newReference = new CachedYingletReference(newFilePath, serializedData, LocalYingletGroup.Custom);
			_yingletRepository.AddNewCustom(newReference);

			OnSaved(Path.GetFileName(newFilePath));

			return newReference;
		}

		public int Delete(CachedYingletReference reference)
		{
			var filePath = reference.Path;

			// Delete the file off disk
			File.Delete(filePath);

			// Remove the reference
			int index = _yingletRepository.DeleteCustom(reference);

			// Edge case: Undo of a delete action
			if (index == -1) return index;

			// Fire the OnDeleted event
			OnDeleted(Path.GetFileName(filePath));

			return index;

		}

		public IEnumerable<CachedYingletReference> LoadInitialCustomYingData()
		{
			var filePaths = GetCustomYingPaths();
			var references = filePaths
				.Select(path => new CachedYingletReference(path, LoadData(path), LocalYingletGroup.Custom))
				.ToList();

			return references;
		}
		IEnumerable<string> GetCustomYingPaths()
		{
			var folder = _locationProvider.CustomFolderRoot;
			return Directory.GetFiles(folder, $"*{EXTENSION}", SearchOption.TopDirectoryOnly);

		}
		SerializableCustomizationData LoadData(string filePath)
		{
			string text = File.ReadAllText(filePath);
			var data = SerializableCustomizationData.FromJSON(text);
			if (data == null)
			{
				Debug.LogError($"Failed to read yinglet at path {filePath}");
			}
			return data;
		}

		string GetUniqueAlphanumericFilePath(string newYingletName, string lastFilePath, string folderPath)
		{
			// Step 1: Make string alphanumeric
			string baseName = Regex.Replace(newYingletName, "[^a-zA-Z0-9]", "");

			if (string.IsNullOrWhiteSpace(baseName))
			{
				baseName = "unnamed";
			}

			// Step 2: Prepare full file path
			string fileName = baseName + EXTENSION;
			string fullPath = Path.Combine(folderPath, fileName);

			int counter = 1;
			while (File.Exists(fullPath))
			{
				if (fullPath == lastFilePath)
				{
					break; // Same name as last time; we're good here
				}

				fileName = $"{baseName}_{counter}{EXTENSION}";
				fullPath = Path.Combine(folderPath, fileName);
				counter++;
			}

			return fullPath;
		}

		void WriteToDisk(string newFilePath, SerializableCustomizationData serializedData)
		{
			string json = JsonUtility.ToJson(serializedData, true);
			File.WriteAllText(newFilePath, json);
		}
	}
}
