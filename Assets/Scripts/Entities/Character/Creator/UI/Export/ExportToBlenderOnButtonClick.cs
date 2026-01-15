using Character.Compositor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class ExportToBlenderOnButtonClick : ExportOnButtonClickBase
{
	private IMaterialGeneration _materialGeneration;
	private IMeshGatherer _meshGatherer;
	private IConfirmationManager _confirmationManager;

	protected override void Awake()
	{
		base.Awake();
		_materialGeneration = this.GetCharacterCreatorComponent<IMaterialGeneration>();
		_meshGatherer = this.GetCharacterCreatorComponent<IMeshGatherer>();

		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
	}

	protected override void OnExportButtonClicked()
	{
		ExecuteExport();

		// Don't really need a confirmation now that we have a real export dialog
		//_confirmationManager.OpenConfirmation(new(
		//	"Export model + textures to Blender?\nThis feature has limited support and\nrequires additional work.",
		//	"Export",
		//	"blender-export",
		//	ExecuteExport));
	}

	void ExecuteExport()
	{
		var selected = _selection.Selected;
		if (selected == null)
		{
			Debug.LogError("No yinglet selected for export");
			return;
		}

		var alphaNumericName = Regex.Replace(selected.CachedData.Name, "[^a-zA-Z0-9_-]", "");
		var folderName = alphaNumericName + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		var newFolder = GetSavePath();

		PathUtils.EnsureDirectoryExists(newFolder);
		Character.Creator.UI.OpenFolderOnButtonClick.OpenFolder(newFolder);

		foreach (var kvp in _materialGeneration.GeneratedMaterialLookup)
		{
			var material = kvp.Value;
			var textures = GetAllTextures(material);
			foreach (var namedTex in textures)
			{
				var matName = kvp.Key.name;
				var texName = namedTex.Name;

				// Early continue: skip legacy _MainTex if _BaseMap exists
				if (texName == "_MainTex")
				{
					if (textures.Any(t => t.Name == "_BaseMap"))
					{
						continue;
					}
				}
				texName = texName.Replace("_MainTex", "_BaseMap");

				// Only export one eye texture
				if (matName.Contains("Eye_Left"))
				{
					continue;
				}
				matName = matName.Replace("Eye_Right", "Eye");

				var name = $"{matName}{namedTex.Name}.png";
				var texPath = Path.Combine(newFolder, name);
				var pngData = ConvertToPNG(namedTex.Texture);
				File.WriteAllBytes(texPath, pngData);
			}
		}

		// Export a manifest of meshes and materials
		ExportManifest(newFolder);

		// Export a readme
		var readmeSB = new StringBuilder();
		File.WriteAllText(Path.Combine(newFolder, "_README.txt"), "For information, go here: https://github.com/TBartl/YingletCreator/wiki/10.-Exporting-a-Yinglet");

		// Export a .blend file to work with
		string blendSourcePath = Path.Combine(Application.streamingAssetsPath, "Yinglet.blend");
		File.Copy(blendSourcePath, Path.Combine(newFolder, "Yinglet.blend"));

		EmitExportEvent();
	}

	void ExportManifest(string newFolder)
	{
		var sb = new StringBuilder();
		foreach (var meshWithMat in _meshGatherer.AllRelevantMeshes)
		{
			sb.AppendLine($"{meshWithMat.SkinnedMeshRendererPrefab.name},{meshWithMat.MaterialDescription.name}");
		}
		File.WriteAllText(Path.Combine(newFolder, "manifest.txt"), sb.ToString());
	}


	static List<NamedTexture> GetAllTextures(Material mat)
	{
		var list = new List<NamedTexture>();

		int count = mat.shader.GetPropertyCount();
		for (int i = 0; i < count; i++)
		{
			if (mat.shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Texture)
			{
				string name = mat.shader.GetPropertyName(i);
				Texture tex = mat.GetTexture(name);
				if (tex != null)
				{
					list.Add(new NamedTexture(name, tex));
				}
			}
		}

		return list;
	}

	sealed class NamedTexture
	{
		public string Name { get; }
		public Texture Texture { get; }
		public NamedTexture(string name, Texture texture)
		{
			Name = name;
			Texture = texture;
		}
	}

	public static byte[] ConvertToPNG(Texture texture)
	{
		// Create a temp RenderTexture the same size
		RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);

		// Copy the texture into the RT
		Graphics.Blit(texture, rt);

		// Activate it
		RenderTexture prev = RenderTexture.active;
		RenderTexture.active = rt;

		// Read pixels into Texture2D
		Texture2D tex2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
		tex2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
		tex2D.Apply();

		RenderTexture.active = prev;
		RenderTexture.ReleaseTemporary(rt);

		// Convert to PNG bytes
		return tex2D.EncodeToPNG();
	}
}
