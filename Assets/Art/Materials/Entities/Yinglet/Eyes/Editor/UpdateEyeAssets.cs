using Character.Compositor;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This script used to do a lot more work, when each expression was separate
/// It would stitch them all together
/// Now it just generates the scriptable objects for the eyes
/// Which could honestly be done manually but whatever I'll take a little automation
/// </summary>
public class UpdateEyeAssets
{
	const string RAW_TEXTURE_PATH = "Assets/Art/Materials/Entities/Yinglet/Eyes/RawTextures";
	const string SCRIPTABLE_OBJECT_OUTPUT_PATH = "Assets/ScriptableObjects/CharacterCompositor/Eyes/";

	[MenuItem("Custom/Update Eye Assets")]
	static void Apply()
	{
		var eyeTextureFolders = Directory.GetDirectories(RAW_TEXTURE_PATH);

		foreach (var eyeTextureFolder in eyeTextureFolders)
		{
			var destinationName = Path.GetFileName(eyeTextureFolder);
			var destinationPath = Path.Combine(SCRIPTABLE_OBJECT_OUTPUT_PATH, $"AddEye-{destinationName}.asset");
			var asset = AssetDatabase.LoadAssetAtPath<AddEyeMixTextures>(destinationPath);

			if (asset == null)
			{
				asset = ScriptableObject.CreateInstance<AddEyeMixTextures>();
				AssetDatabase.CreateAsset(asset, destinationPath);
			}

			var fill = LoadTex(Path.Combine(eyeTextureFolder, "Fill.png"));
			var eyelid = LoadTex(Path.Combine(eyeTextureFolder, "Eyelid.png"));
			bool eyelidContainsNonBlackPixel = ContainsNonBlackPixel(eyelid);
			asset.EditorSetTextures(fill, eyelid, eyelidContainsNonBlackPixel);

			EditorUtility.SetDirty(asset);
			AssetDatabase.SaveAssets();
		}
	}

	static Texture2D LoadTex(string path)
	{
		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
		if (importer == null)
		{
			Debug.LogWarning($"Failed to find expected importer at {path}");
			return null;
		}
		importer.isReadable = true;
		importer.alphaIsTransparency = true;
		AssetDatabase.ImportAsset(path);

		var tex2d = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		if (tex2d == null)
		{
			Debug.LogWarning($"Failed to find expected tex2d at {path}");
			return null;
		}
		return tex2d;
	}

	static bool ContainsNonBlackPixel(Texture2D tex)
	{
		var pixels = tex.GetPixels();
		foreach (var pixel in pixels)
		{
			if (pixel.r > 0.5f && pixel.a > 0.5f)
			{
				return true;
			}
		}
		return false;

	}
}
