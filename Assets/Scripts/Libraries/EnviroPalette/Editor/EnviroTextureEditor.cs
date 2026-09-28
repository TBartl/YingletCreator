using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnviroTexture))]
public class EnviroTextureEditor : Editor
{
	private Editor rampEditor;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();

		var enviroTexture = (EnviroTexture)target;

		// Display the generated texture if it exists
		if (enviroTexture.Texture != null)
		{
			EditorGUILayout.LabelField("Original -> Paletted");

			EditorGUILayout.BeginHorizontal();
			{
				Rect previewRect = EditorGUILayout.GetControlRect(GUILayout.Height(128), GUILayout.Width(128));
				EditorGUI.DrawPreviewTexture(previewRect, enviroTexture.Texture);

				GUILayout.Space(10);

				if (enviroTexture.Generated != null)
				{
					Rect previewRectGenerated = EditorGUILayout.GetControlRect(GUILayout.Height(128), GUILayout.Width(128));
					EditorGUI.DrawPreviewTexture(previewRectGenerated, enviroTexture.Generated);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();

		if (GUILayout.Button("Generate Texture"))
		{
			GenerateTexture(enviroTexture);
		}

		// Draw the associated EnviroRamp inspector
		if (enviroTexture.Ramp != null)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Ramp Settings", EditorStyles.boldLabel);

			if (rampEditor == null)
			{
				rampEditor = CreateEditor(enviroTexture.Ramp);
			}

			rampEditor.OnInspectorGUI();
		}
	}

	private void OnDisable()
	{
		if (rampEditor != null)
		{
			DestroyImmediate(rampEditor);
		}
	}

	public static void GenerateTexture(EnviroTexture enviroTexture)
	{
		if (enviroTexture.Ramp == null)
		{
			Debug.LogError("EnviroTexture must have a Ramp assigned before generating.", enviroTexture);
			return;
		}
		if (enviroTexture.Texture == null)
		{
			Debug.LogError("EnviroTexture must have a Texture assigned before generating.", enviroTexture);
			return;
		}

		string texturePath = GetOutputTexturePath(enviroTexture);

		// Generate new texture if source texture is provided
		Texture2D generatedTexture = RenderTextureWithRamp(enviroTexture);
		SaveTextureAsset(generatedTexture, texturePath);
		Object.DestroyImmediate(generatedTexture);
		ConfigureTextureImporter(texturePath);

		UpdateScriptableWithGenerated(enviroTexture, texturePath);

		Debug.Log($"Generated texture: {texturePath}", enviroTexture);
	}

	static string GetOutputTexturePath(EnviroTexture enviroTexture)
	{
		string assetPath = AssetDatabase.GetAssetPath(enviroTexture);
		string directory = System.IO.Path.GetDirectoryName(assetPath);
		return $"{directory}/{enviroTexture.name}_Generated.png";
	}

	static Texture2D RenderTextureWithRamp(EnviroTexture enviroTexture)
	{
		var material = CreateColorizeMaterial(enviroTexture);
		var texture = new Texture2D(enviroTexture.Texture.width, enviroTexture.Texture.height, TextureFormat.RGBA32, false, true);
		texture.wrapMode = TextureWrapMode.Mirror;
		texture.filterMode = FilterMode.Bilinear;

		BlitToTexture(enviroTexture.Texture, texture, material);

		Object.DestroyImmediate(material);
		return texture;
	}

	static Material CreateColorizeMaterial(EnviroTexture enviroTexture)
	{
		var material = new Material(Shader.Find("EnviroColorize"));
		material.SetTexture("_MainTex", enviroTexture.Texture);
		material.SetTexture("_RampTex", enviroTexture.Ramp.Generated);
		material.SetFloat("_HueOffset", enviroTexture.Hue);
		material.SetFloat("_HueInfluence", enviroTexture.HueInfluence);
		return material;
	}

	static void BlitToTexture(Texture2D sourceTexture, Texture2D targetTexture, Material material)
	{
		var rt = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0, RenderTextureFormat.ARGB32);
		Graphics.Blit(sourceTexture, rt, material);

		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = rt;
		targetTexture.ReadPixels(new Rect(0, 0, sourceTexture.width, sourceTexture.height), 0, 0);
		targetTexture.Apply();
		RenderTexture.active = previous;

		RenderTexture.ReleaseTemporary(rt);
	}

	static void SaveTextureAsset(Texture2D texture, string texturePath)
	{
		byte[] png = texture.EncodeToPNG();

		System.IO.File.WriteAllBytes(texturePath, png);
		AssetDatabase.ImportAsset(texturePath);
	}

	static void ConfigureTextureImporter(string texturePath)
	{
		var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
		if (importer == null)
		{
			Debug.LogError($"Failed to get TextureImporter for {texturePath}");
			return;
		}

		importer.textureCompression = TextureImporterCompression.CompressedHQ;
		importer.wrapMode = TextureWrapMode.Clamp;
		importer.filterMode = FilterMode.Bilinear;
		importer.mipmapEnabled = false;
		importer.isReadable = false;

		importer.SaveAndReimport();
	}

	static void UpdateScriptableWithGenerated(EnviroTexture enviroTexture, string texturePath)
	{
		var generatedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
		if (generatedTexture == null)
		{
			Debug.LogError($"Failed to load generated texture at {texturePath}");
			return;
		}

		enviroTexture.Generated = generatedTexture;
		EditorUtility.SetDirty(enviroTexture);
		AssetDatabase.SaveAssets();
	}
}
