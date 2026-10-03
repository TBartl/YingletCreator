using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnviroTexture))]
public class EnviroTextureEditor : Editor
{
	private Editor _rampEditor;
	private EnviroRamp _cachedRamp;
	private List<Editor> _maskLayerRampEditors;
	private EnviroMaskLayer[] _cachedMaskLayers;

	private void OnEnable()
	{
		_maskLayerRampEditors = new List<Editor>();
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();

		var enviroTexture = (EnviroTexture)target;

		if (GUILayout.Button("Update source color range"))
		{
			UpdateColorRange(enviroTexture);
		}

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
			EnsureRampEditor(enviroTexture);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Ramp Settings", EditorStyles.boldLabel);

			_rampEditor.OnInspectorGUI();

			if (enviroTexture.MaskLayers != null && enviroTexture.MaskLayers.Length > 0)
			{
				EnsureMaskLayerEditors(enviroTexture.MaskLayers);

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Mask Layers", EditorStyles.boldLabel);

				for (int i = 0; i < enviroTexture.MaskLayers.Length; i++)
				{
					var maskLayer = enviroTexture.MaskLayers[i];
					if (maskLayer.Ramp != null && i < _maskLayerRampEditors.Count)
					{
						EditorGUILayout.Space();
						_maskLayerRampEditors[i].OnInspectorGUI();
					}
				}
			}
		}
	}

	/// <summary>
	/// Generates the ramp editor if null or has changed
	/// </summary>
	void EnsureRampEditor(EnviroTexture enviroTexture)
	{
		if (_cachedRamp == enviroTexture.Ramp) return; // Unchanged

		if (_rampEditor != null)
		{
			DestroyImmediate(_rampEditor);
		}
		_rampEditor = CreateEditor(enviroTexture.Ramp);
		_cachedRamp = enviroTexture.Ramp;
	}

	void EnsureMaskLayerEditors(EnviroMaskLayer[] maskLayers)
	{
		if (_cachedMaskLayers != null && _cachedMaskLayers.SequenceEqual(maskLayers)) return; // Unchanged

		// Destroy existing editors
		foreach (var editor in _maskLayerRampEditors)
		{
			if (editor != null)
			{
				DestroyImmediate(editor);
			}
		}
		_maskLayerRampEditors.Clear();

		// Create new editors if mask layers exist
		if (maskLayers != null)
		{
			foreach (var maskLayer in maskLayers)
			{
				if (maskLayer.Ramp != null)
				{
					_maskLayerRampEditors.Add(CreateEditor(maskLayer.Ramp));
				}
			}
		}

		_cachedMaskLayers = maskLayers.ToArray();
	}

	private void OnDisable()
	{
		if (_rampEditor != null)
		{
			DestroyImmediate(_rampEditor);
		}

		foreach (var editor in _maskLayerRampEditors)
		{
			DestroyImmediate(editor);
		}
		_maskLayerRampEditors.Clear();
	}

	public static void UpdateColorRange(EnviroTexture enviroTexture)
	{
		using var writeable = TexGenerationUtils.MakeTemporarilyWriteable(enviroTexture.Texture);
		Color[] pixels = enviroTexture.Texture.GetPixels();

		if (pixels.Length == 0)
			return;

		System.Array.Sort(pixels, (a, b) =>
			a.grayscale.CompareTo(b.grayscale));

		enviroTexture.ColorRange.MinColor = pixels[0];
		enviroTexture.ColorRange.MidColor = pixels[pixels.Length / 2]; // Median
		enviroTexture.ColorRange.MaxColor = pixels[pixels.Length - 1];
		EditorUtility.SetDirty(enviroTexture);
		AssetDatabase.SaveAssets();
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
		ConfigureTextureImporter(texturePath, enviroTexture.Texture);

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
		// Setup the target texture
		var targetTexture = new Texture2D(enviroTexture.Texture.width, enviroTexture.Texture.height, TextureFormat.RGBA32, false, true);
		targetTexture.wrapMode = TextureWrapMode.Repeat;
		targetTexture.filterMode = FilterMode.Bilinear;

		// Setup the render textures to write between
		using var renderTextures = new DoubleBufferedRenderTexture(new Vector2Int(targetTexture.width, targetTexture.height));

		// Setup the material
		var material = CreateColorizeMaterial(enviroTexture);

		// Apply main texture
		material.SetTexture("_RampTex", enviroTexture.Ramp.Generated);
		renderTextures.Blit(material);

		// Apply additional mask layers
		foreach (var maskLayer in enviroTexture.MaskLayers)
		{
			material.SetTexture("_MaskTex", maskLayer.Texture);
			material.SetTexture("_RampTex", maskLayer.Ramp.Generated);
			renderTextures.Blit(material);
		}

		// Read from the render texture into the target texture
		var final = renderTextures.Finalize();
		RenderTexture.active = final;
		targetTexture.ReadPixels(new Rect(0, 0, enviroTexture.Texture.width, enviroTexture.Texture.height), 0, 0);
		targetTexture.Apply();

		// Cleanup
		RenderTexture.active = null;
		Object.DestroyImmediate(material);
		return targetTexture;
	}

	static Material CreateColorizeMaterial(EnviroTexture enviroTexture)
	{
		var material = new Material(Shader.Find("EnviroColorize"));
		material.SetTexture("_SampleTex", enviroTexture.Texture);
		material.SetFloat("_HueInfluence", enviroTexture.HueInfluence);
		material.SetColor("_MinColor", enviroTexture.ColorRange.MinColor);
		material.SetColor("_MidColor", enviroTexture.ColorRange.MidColor);
		material.SetColor("_MaxColor", enviroTexture.ColorRange.MaxColor);
		material.SetFloat("_AllowBeyondRange", enviroTexture.AdvancedSettings.AllowBeyondRange ? 1f : 0f);
		material.SetFloat("_BelowRangeMultiplier", enviroTexture.AdvancedSettings.BelowRangeMultiplier);
		material.SetFloat("_AboveRangeMultiplier", enviroTexture.AdvancedSettings.AboveRangeMultiplier);
		return material;
	}

	static void SaveTextureAsset(Texture2D texture, string texturePath)
	{
		byte[] png = texture.EncodeToPNG();

		System.IO.File.WriteAllBytes(texturePath, png);
		AssetDatabase.ImportAsset(texturePath);
	}

	static void ConfigureTextureImporter(string texturePath, Texture2D source)
	{
		var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
		if (importer == null)
		{
			Debug.LogError($"Failed to get TextureImporter for {texturePath}");
			return;
		}

		importer.textureCompression = TextureImporterCompression.CompressedHQ;
		importer.wrapMode = source.wrapMode;
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
