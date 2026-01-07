using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelMaterial : ModelItem
{
	public static Texture2D pupilTexture;

	public Material unityMaterial;
	public Texture2D unityTexture;
	public int textureIndex = -1;
	public byte[] pngBytes;
	public bool isTargetOfTextureAnimation = false;

	public static int FromSingleMaterial(ModelDocument doc, Material unityMaterial)
	{
		Texture2D tex2D = MaterialMainTextureToTexture2D(unityMaterial);
		if (tex2D is null)
		{
			Debug.LogError("Material " + unityMaterial.name + " failed to have its main texture converted to Texture2D.");
			return -1;
		}
		byte[] pngBytes = tex2D.EncodeToPNG();
		for (int i = 0; i < doc.materials.Count; i++)
		{
			if (doc.materials[i].unityMaterial == unityMaterial)
			{
				return i;
			}
			if (doc.materials[i].pngBytes.SequenceEqual(pngBytes))
			{
				return i;
			}
		}
		ModelMaterial modelMaterial = new ModelMaterial();
		modelMaterial.name = unityMaterial.name.Replace(" (Instance)", "");
		modelMaterial.name = doc.ReserveUniqueName(modelMaterial.name);
		modelMaterial.pngBytes = pngBytes;
		modelMaterial.unityMaterial = unityMaterial;
		modelMaterial.unityTexture = tex2D;
		int matIndex = doc.materials.Count;
		doc.materials.Add(modelMaterial);
		return matIndex;
	}

	public int DuplicateAndAddModelMaterial(ModelDocument doc, string requestedName)
	{
		ModelMaterial duplicated = new ModelMaterial();
		duplicated.isTargetOfTextureAnimation = isTargetOfTextureAnimation;
		duplicated.name = doc.ReserveUniqueName(requestedName);
		if (pngBytes != null)
		{
			duplicated.pngBytes = (byte[])pngBytes.Clone();
		}
		duplicated.textureIndex = textureIndex;
		duplicated.unityMaterial = unityMaterial;
		duplicated.unityTexture = unityTexture;
		int matIndex = doc.materials.Count;
		doc.materials.Add(duplicated);
		return matIndex;
	}

	public static ModelMaterial AtlasMultipleMaterials(SkinnedMeshRenderer[] skinnedMeshes, ModelMesh.ModelMeshSurface singleSurface, ref Rect leftEyeRect, ref Rect rightEyeRect, ref Rect pupilRect)
	{
		List<Material> uniqueMaterials = new List<Material>();
		Dictionary<Material, int> uniqueMaterialToIndexMap = new Dictionary<Material, int>();
		for (int i = 0; i < skinnedMeshes.Length; i++)
		{
			Material mat = skinnedMeshes[i].sharedMaterial;
			if (!uniqueMaterialToIndexMap.ContainsKey(mat))
			{
				uniqueMaterialToIndexMap[mat] = uniqueMaterials.Count;
				uniqueMaterials.Add(mat);
			}
		}
		List<Texture2D> texture2Ds = new List<Texture2D>();
		for (int i = 0; i < uniqueMaterials.Count; i++)
		{
			Material mat = uniqueMaterials[i];
			Texture2D tex2D;
			// Special case: Eye material needs to have the eyelids composed.
			if (mat.name == "Mat_Eye")
			{
				tex2D = EyeMainMaterialToTexture2D(mat);
			}
			else
			{
				tex2D = MaterialMainTextureToTexture2D(mat);
			}
			texture2Ds.Add(tex2D);
		}
		// Special case: The pupil texture also goes into this atlas.
		int pupilTextureIndex = texture2Ds.Count;
		texture2Ds.Add(pupilTexture);
		Texture2D atlas = new Texture2D(512, 512);
		Rect[] rects = atlas.PackTextures(
			texture2Ds.ToArray(),
			0,
			512,
			false
		);
		pupilRect = rects[pupilTextureIndex];
		// Pack UVs to match the atlas.
		singleSurface.materialIndex = 0; // Since we are atlasing into a single material.
		List<Vector2> textureMap = new List<Vector2>();
		for (int smi = 0; smi < skinnedMeshes.Length; smi++)
		{
			SkinnedMeshRenderer skinnedMesh = skinnedMeshes[smi];
			Material mat = skinnedMesh.sharedMaterial;
			int uniqueMatIndex = uniqueMaterialToIndexMap[mat];
			Rect rect = rects[uniqueMatIndex];
			// Special case: The eyes have their textures in this atlas too, but not in the material surface.
			if (mat.name == "Mat_Eye")
			{
				if (skinnedMesh.name == "Eye-Left")
				{
					leftEyeRect = rect;
				}
				else if (skinnedMesh.name == "Eye-Right")
				{
					rightEyeRect = rect;
				}
				continue;
			}
			RefitTextureMapIntoRect(textureMap, skinnedMesh, rect);
		}
		singleSurface.textureMap = textureMap;
		// Create ModelMaterial.
		ModelMaterial modelMaterial = new ModelMaterial();
		modelMaterial.unityTexture = atlas;
		return modelMaterial;
	}

	public static void RefitTextureMapIntoRect(List<Vector2> destTextureMap, SkinnedMeshRenderer skinnedMesh, Rect rect)
	{
		Mesh bakedMesh = new Mesh();
		skinnedMesh.BakeMesh(bakedMesh);
		Vector2[] sourceUVs = bakedMesh.uv;
		for (int uvi = 0; uvi < sourceUVs.Length; uvi++)
		{
			Vector2 uv = sourceUVs[uvi];
			// Remap UV to atlas rect.
			Vector2 remappedUV = new Vector2(
				rect.xMin + uv.x * rect.width,
				rect.yMin + uv.y * rect.height
			);
			destTextureMap.Add(remappedUV);
		}
	}

	private static Texture2D MaterialMainTextureToTexture2D(Material mat)
	{
		if (mat.mainTexture is Texture2D)
		{
			return mat.mainTexture as Texture2D;
		}
		RenderTexture mainTex = mat.mainTexture as RenderTexture;
		return RenderTextureToTexture2D(mainTex);
	}

	public static Texture2D EyeMainMaterialToTexture2D(Material mat)
	{
		RenderTexture mainTex = mat.mainTexture as RenderTexture;
		RenderTexture eyelidTex = mat.GetTexture("_Eyelid") as RenderTexture;
		// Convert both textures to Texture2D.
		Texture2D baseTex = RenderTextureToTexture2D(mainTex);
		Texture2D eyelidTex2D = RenderTextureToTexture2D(eyelidTex);
		// Manually blend pixels with alpha compositing
		Color[] basePixels = baseTex.GetPixels();
		Color[] eyelidPixels = eyelidTex2D.GetPixels();
		for (int i = 0; i < basePixels.Length; i++)
		{
			Color baseColor = basePixels[i];
			Color eyelidColor = eyelidPixels[i];
			float alpha = eyelidColor.a;
			// Standard alpha compositing: result = src * alpha + dst * (1 - alpha)
			basePixels[i] = new Color(
				eyelidColor.r * alpha + baseColor.r * (1 - alpha),
				eyelidColor.g * alpha + baseColor.g * (1 - alpha),
				eyelidColor.b * alpha + baseColor.b * (1 - alpha),
				Mathf.Max(baseColor.a, eyelidColor.a)
			);
		}
		baseTex.SetPixels(basePixels);
		baseTex.Apply();
		Object.DestroyImmediate(eyelidTex2D);
		return baseTex;
	}

	private static Texture2D RenderTextureToTexture2D(RenderTexture renderTex)
	{
		Texture2D tex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
		RenderTexture currentActiveRT = RenderTexture.active;
		RenderTexture.active = renderTex;
		tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
		tex.Apply();
		RenderTexture.active = currentActiveRT;
		return tex;
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{");
		if (format == ModelBaseFormat.GLTF)
		{
			json.Append("\"alphaMode\":\"MASK\",");
			json.Append("\"extensions\":{");
			json.Append("\"KHR_materials_unlit\":{}");
			//json.Append(",\"VRMC_materials_mtoon\":{\"specVersion\":\"1.0\"}");
			json.Append("}"); // End extensions
			json.Append(",\"name\":\"" + name + "\"");
			json.Append(",\"pbrMetallicRoughness\":{\"baseColorTexture\":{");
			if (isTargetOfTextureAnimation)
			{
				json.Append("\"extensions\":{\"KHR_texture_transform\":{\"offset\":[0.0,0.0]}},");
			}
			json.Append("\"index\":");
			json.Append(textureIndex);
			json.Append("}"); // End baseColorTexture
			json.Append("}"); // End pbrMetallicRoughness
		}
		else
		{
			json.Append("\"baseColor\":{");
			if (isTargetOfTextureAnimation)
			{
				json.Append("\"extensions\":{\"KHR_texture_transform\":{\"offset\":[0.0,0.0]}},");
			}
			json.Append("\"texture\":");
			json.Append(textureIndex);
			json.Append("}"); // End baseColor
			json.Append(",\"extensions\":{");
			json.Append("\"KHR_materials_unlit\":{}");
			//json.Append(",\"VRMC_materials_mtoon\":{\"specVersion\":\"1.0\"}");
			json.Append("}"); // End extensions
			json.Append(",\"name\":\"" + name + "\"");
		}
		json.Append("}");
		return json.ToString();
	}
}
