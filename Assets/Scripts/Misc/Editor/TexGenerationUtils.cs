using System;
using UnityEditor;
using UnityEngine;

public static class TexGenerationUtils
{

	public static void UpdateTextureSettings(string path, bool compress)
	{
		var importer = AssetImporter.GetAtPath(path) as TextureImporter;
		if (importer == null) return; // This asset may not exist
		importer.isReadable = true;
		importer.alphaIsTransparency = true;
		importer.compressionQuality = compress ? 3 : 100; // Source textures shouldn't be compressed
		importer.sRGBTexture = true; // These are used for grayscale
		importer.SaveAndReimport();
	}

	public static Color[] CreateTransparentPixels(Vector2Int size)
	{
		Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);

		Color transparentColor = new Color(0, 0, 0, 0); // Fully transparent
		Color[] pixels = new Color[size.x * size.y];

		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = transparentColor;
		}

		return pixels;
	}

	public static IDisposable MakeTemporarilyWriteable(Texture2D texture)
	{
		var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
		if (importer == null) throw new Exception("Texture is not an asset or cannot be found.");
		bool originalIsReadable = importer.isReadable;

		if (originalIsReadable)
		{
			// Already readable, no need to change anything
			return new BasicActionDisposable(() => { });
		}

		importer.isReadable = true;
		importer.SaveAndReimport();
		return new BasicActionDisposable(() =>
		{
			importer.isReadable = originalIsReadable;
			importer.SaveAndReimport();
		});
	}
}
