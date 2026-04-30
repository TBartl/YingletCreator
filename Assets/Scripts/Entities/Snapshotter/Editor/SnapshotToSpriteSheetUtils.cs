
using Character.Creator;
using Character.Creator.UI;
using Character.Data;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Snapshotter
{
	public static class SnapshotToSpriteSheetUtils
	{
		const string ReferencesRelativePath = "Assets/Scripts/Entities/Snapshotter/SnapshotterReferences_ToggleIcons.asset";
		const string CameraPosRelativePath = "Assets/ScriptableObjects/CharacterCompositor/SnapshotterCameraPositions/_Default.asset";
		const string PresetPath = "Assets/Scripts/Entities/Snapshotter/SnapshotYing.yingsave";

		public static void GenerateToggleIcons(ModDefinition modDefinition)
		{
			const string OutputName = "GeneratedToggleIcons.png";
			string outputFolder = modDefinition.GetParentFolder();
			string outputPath = Path.Combine(outputFolder, OutputName);
			var toggles = ObjectExtensionMethods.LoadAllAssets<CharacterToggleId>(outputFolder).ToArray();
			// Filter out toggles that we don't want an icon for
			toggles = toggles.Where(t => !t.Components.Any(c => c is NoToggleIcon)).ToArray();
			SnapshotToTexAndApply(toggles, outputPath);
		}

		public static void GeneratePoseIcons(ModDefinition modDefinition)
		{
			const string OutputName = "GeneratedPoseIcons.png";
			string outputFolder = modDefinition.GetParentFolder();
			string outputPath = Path.Combine(outputFolder, OutputName);
			var poses = ObjectExtensionMethods.LoadAllAssets<PoseId>(outputFolder).ToArray();
			poses = poses.Where(p => p.Order.Group != null).ToArray();
			SnapshotToTexAndApply(poses, outputPath);
		}

		public static void GeneratePortraitIcons(ModDefinition modDefinition)
		{
			const string OutputName = "GeneratedPortraitIcons.png";
			string outputFolder = modDefinition.GetParentFolder();
			string outputPath = Path.Combine(outputFolder, OutputName);
			var portraits = ObjectExtensionMethods.LoadAllAssets<PortraitId>(outputFolder).ToArray();
			SnapshotToTexAndApply(portraits, outputPath);
		}

		static void SnapshotToTexAndApply(ISnapshottableScriptableObject[] snapshottables, string outputPath)
		{
			if (!EditorApplication.isPlaying)
			{
				Debug.LogError("Snapshotting logic can only be ran while playing");
				return;
			}
			if (!snapshottables.Any())
			{
				return;
			}

			var resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
			if (resourceLoader == null)
			{
				Debug.LogError("Missing dependencies required to snapshot; are you in the CharacterCreator scene?");
				return;
			}

			Debug.Log($"Generating icons for {snapshottables.Length} snapshottables");
			var references = AssetDatabase.LoadAssetAtPath<SnapshotterReferences>(ReferencesRelativePath);
			int totalTexSize = CalculateTotalTextureSize(references.SizeInPixels, snapshottables.Length);

			SnapshotsToTexture(references, snapshottables, outputPath, totalTexSize, resourceLoader);
			SpliceSpriteSheet(references, snapshottables, outputPath, totalTexSize);
			ApplySpriteSheetToSnapshottables(snapshottables, outputPath);
		}

		static void SnapshotsToTexture(SnapshotterReferences references, ISnapshottableScriptableObject[] snapshottables, string outputPath, int totalTexSize, ICompositeResourceLoader resourceLoader)
		{
			var defaultCamPos = AssetDatabase.LoadAssetAtPath<SnapshotterCameraPosition>(CameraPosRelativePath);

			string text = File.ReadAllText(PresetPath);
			var serializedData = JsonUtility.FromJson<SerializableCustomizationData>(text);

			Debug.Log($"Texture size required: {totalTexSize}x{totalTexSize}");


			Texture2D tex = new Texture2D(totalTexSize, totalTexSize, TextureFormat.RGBA32, false);
			MakeTransparent(tex);

			for (int i = 0; i < snapshottables.Length; i++)
			{
				var snapshottable = snapshottables[i];
				EditorUtility.DisplayProgressBar("Snapshot Icon Previews", $"Generating icon ({i}/{snapshottables.Length}) - '{snapshottable.DisplayName}'", i / (float)snapshottables.Length);

				var observableData = new ObservableCustomizationData(serializedData, resourceLoader);

				var camPos = snapshottable.Preview.CameraPosition ?? defaultCamPos;
				var sParams = new SnapshotterParams(camPos, observableData);

				// Kinda hacky casting but w/e
				if (snapshottable is CharacterToggleId toggle)
				{
					if (!observableData.GetToggle(toggle)) // Accounts for the defaults
					{
						observableData.FlipToggle(toggle);
					}
				}
				if (snapshottable is PoseId pose)
				{
					sParams.Pose = pose;
				}
				if (snapshottable is PortraitId portrait)
				{
					sParams.Portrait = portrait;
					observableData.PortraitData.PortraitId.Val = portrait;
				}

				var rt = SnapshotterUtils.Snapshot(references, sParams);

				// Create Texture2D and read pixels
				RenderTexture.active = rt;
				var offset = GetOffsetFromIndex(i, references.SizeInPixels, totalTexSize);
				tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), offset.x, offset.y);
				tex.Apply();
				RenderTexture.active = null;
				GameObject.DestroyImmediate(rt);
			}
			EditorUtility.ClearProgressBar();

			// Encode to PNG
			byte[] pngData = tex.EncodeToPNG();
			File.WriteAllBytes(outputPath, pngData);
			Debug.Log("Saved PNG to: " + outputPath);

			// Refresh AssetDatabase to show the new file
			AssetDatabase.Refresh();

			SetTextureToSprite(outputPath);

			// Cleanup
			GameObject.DestroyImmediate(tex);
		}

		static void SpliceSpriteSheet(SnapshotterReferences references, ISnapshottableScriptableObject[] snapshottables, string outputPath, int totalTexSize)
		{
			SpriteRect[] spriteRects = new SpriteRect[snapshottables.Length];
			int index = 0;

			for (int i = 0; i < snapshottables.Length; i++)
			{
				var snapshottable = snapshottables[i];
				var offset = GetOffsetFromIndex(i, references.SizeInPixels, totalTexSize);

				SpriteRect meta = new SpriteRect();
				meta.rect = new Rect(offset.x, offset.y, references.SizeInPixels, references.SizeInPixels); // note: origin is bottom-left
				meta.name = snapshottable.name;
				meta.pivot = new Vector2(0.5f, 0.5f); // center pivot
				meta.spriteID = GenerateDeterministicSpriteId(snapshottable.UniqueAssetID);
				spriteRects[index++] = meta;
			}

			TextureImporter importer = AssetImporter.GetAtPath(outputPath) as TextureImporter;

			var factory = new SpriteDataProviderFactories();
			factory.Init();
			var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
			dataProvider.InitSpriteEditorDataProvider();
			dataProvider.SetSpriteRects(spriteRects);
			dataProvider.Apply();
			AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceUpdate);
		}

		private static void ApplySpriteSheetToSnapshottables(ISnapshottableScriptableObject[] snapshottables, string outputPath)
		{
			string spriteSheetPath = outputPath;
			var sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath)
				.Select(s => s as Sprite)
				.Where(s => s != null)
				.ToDictionary((sprite) => sprite.name);
			// If this is getting hit, there's a good chance the texture isn't set up as a sprite (should probably do that programatically but w/e)
			Debug.Assert(snapshottables.Length == sprites.Keys.Count());
			for (int i = 0; i < snapshottables.Length; i++)
			{
				var snapshottable = snapshottables[i];
				var sprite = sprites[snapshottable.name];
				snapshottable.Preview.SetSprite(sprite);
				EditorUtility.SetDirty((ScriptableObject)snapshottable);
			}
			AssetDatabase.SaveAssets();
		}

		static void MakeTransparent(Texture2D tex)
		{
			Color32[] pixels = new Color32[tex.width * tex.width];
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = new Color32(0, 0, 0, 0);
			}

			tex.SetPixels32(pixels);
			tex.Apply();
		}

		static int CalculateTotalTextureSize(int cellSize, int itemCount)
		{
			int spaceNeeded = cellSize * cellSize * itemCount;

			int testSize = 128;
			while (spaceNeeded > testSize * testSize)
			{
				testSize *= 2;
			}
			return testSize;
		}

		static Vector2Int GetOffsetFromIndex(int index, int cellSize, int totalSize)
		{
			var totalRows = totalSize / cellSize;
			int xOffset = (index % totalRows) * cellSize;
			int yOffset = (index / totalRows) * cellSize;
			return new Vector2Int(xOffset, yOffset);
		}

		public static GUID GenerateDeterministicSpriteId(string uniqueInput)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(uniqueInput));
				Guid guid = new Guid(hashBytes);
				return new GUID(guid.ToString("N"));
			}
		}

		public static void SetTextureToSprite(string assetPath)
		{
			var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

			if (importer.textureType == TextureImporterType.Sprite && importer.spriteImportMode == SpriteImportMode.Multiple)
			{
				return; // Already good
			}

			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;

			EditorUtility.SetDirty(importer);
			importer.SaveAndReimport();
		}

		public static void UpdateIconsInScene()
		{
			var sprites = UnityEngine.Object.FindObjectsByType<CharacterCreatorToggleSprite>();
			foreach (var sprite in sprites)
			{
				sprite.Start(); // Call to force regeneration
			}
		}
	}
}
