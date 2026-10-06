using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnviroRamp))]
public class EnviroRampEditor : Editor
{
	private const int TextureWidth = 128;
	private const int TextureHeight = 1;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();

		var ramp = (EnviroRamp)this.target;

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("-3% Sat."))
		{
			ModifyGradientSaturation(ramp, -.03f);
		}
		if (GUILayout.Button("-1% Sat."))
		{
			ModifyGradientSaturation(ramp, -.01f);
		}
		if (GUILayout.Button("+1% Sat."))
		{
			ModifyGradientSaturation(ramp, .01f);
		}
		if (GUILayout.Button("+3% Sat."))
		{
			ModifyGradientSaturation(ramp, .03f);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("-3% Bright."))
		{
			ModifyGradientBrightness(ramp, -.03f);
		}
		if (GUILayout.Button("-1% Bright."))
		{
			ModifyGradientBrightness(ramp, -.01f);
		}
		if (GUILayout.Button("+1% Bright."))
		{
			ModifyGradientBrightness(ramp, .01f);
		}
		if (GUILayout.Button("+3% Bright."))
		{
			ModifyGradientBrightness(ramp, .03f);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		if (GUILayout.Button("Generate Ramp Texture"))
		{
			GenerateRampTexture(ramp);
		}

		if (GUILayout.Button("Generate Ramp Texture and Apply to EnviroTextures"))
		{
			GenerateRampTexture(ramp);
			ApplyToEnviroTextures(ramp);
		}
	}

	static void ModifyGradientSaturation(EnviroRamp ramp, float saturationShift)
	{
		Gradient gradient = ramp.Gradient;
		GradientColorKey[] colorKeys = gradient.colorKeys;
		GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

		for (int i = 0; i < colorKeys.Length; i++)
		{
			Color color = colorKeys[i].color;
			Color.RGBToHSV(color, out float h, out float s, out float v);
			s = Mathf.Clamp01(s + saturationShift);
			colorKeys[i].color = Color.HSVToRGB(h, s, v);
		}

		gradient.SetKeys(colorKeys, alphaKeys);
		EditorUtility.SetDirty(ramp);
		AssetDatabase.SaveAssets();
	}

	static void ModifyGradientBrightness(EnviroRamp ramp, float brightnessShift)
	{
		Gradient gradient = ramp.Gradient;
		GradientColorKey[] colorKeys = gradient.colorKeys;
		GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

		for (int i = 0; i < colorKeys.Length; i++)
		{
			Color color = colorKeys[i].color;
			Color.RGBToHSV(color, out float h, out float s, out float v);
			v = Mathf.Clamp01(v + brightnessShift);
			colorKeys[i].color = Color.HSVToRGB(h, s, v);
		}

		gradient.SetKeys(colorKeys, alphaKeys);
		EditorUtility.SetDirty(ramp);
		AssetDatabase.SaveAssets();
	}

	static void GenerateRampTexture(EnviroRamp ramp)
	{
		string assetPath = AssetDatabase.GetAssetPath(ramp);
		string directory = System.IO.Path.GetDirectoryName(assetPath);
		string texturePath = $"{directory}/{ramp.name}_Generated.png";

		var texture = new Texture2D(
			TextureWidth,
			TextureHeight,
			TextureFormat.RGBA32,
			false,
			true);

		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;

		for (int x = 0; x < TextureWidth; x++)
		{
			float t = x / (float)(TextureWidth - 1);
			Color color = ramp.Gradient.Evaluate(t);

			texture.SetPixel(x, 0, color);
		}

		texture.Apply();

		byte[] png = texture.EncodeToPNG();
		Object.DestroyImmediate(texture);

		System.IO.File.WriteAllBytes(texturePath, png);
		AssetDatabase.ImportAsset(texturePath);

		var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
		if (importer == null)
		{
			Debug.LogError($"Failed to get TextureImporter for {texturePath}");
			return;
		}
		importer.textureCompression = TextureImporterCompression.Uncompressed;
		importer.wrapMode = TextureWrapMode.Clamp;
		importer.filterMode = FilterMode.Bilinear;
		importer.mipmapEnabled = false;
		importer.isReadable = false;

		importer.SaveAndReimport();

		var generatedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
		if (generatedTexture == null)
		{
			Debug.LogError($"Failed to load generated texture at {texturePath}");
			return;
		}
		ramp.Generated = generatedTexture;
		EditorUtility.SetDirty(ramp);
		AssetDatabase.SaveAssets();

		Debug.Log($"Generated ramp texture: {texturePath}", ramp);
	}

	void ApplyToEnviroTextures(EnviroRamp ramp)
	{
		string[] guids = AssetDatabase.FindAssets("t:EnviroTexture");

		if (guids.Length == 0)
		{
			Debug.Log("No EnviroTexture assets found in the project.");
			return;
		}

		foreach (string guid in guids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			EnviroTexture enviroTexture = AssetDatabase.LoadAssetAtPath<EnviroTexture>(assetPath);

			if (enviroTexture == null) continue;
			if (enviroTexture.Ramp == ramp || enviroTexture.MaskLayers.Any(l => l.Ramp == ramp))
			{
				EnviroTextureEditor.GenerateTexture(enviroTexture);
			}
		}
	}
}