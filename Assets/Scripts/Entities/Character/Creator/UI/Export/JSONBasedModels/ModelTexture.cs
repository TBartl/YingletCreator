using UnityEngine;

public class ModelTexture : ModelItem
{
	public int bufferViewIndex = -1;
	public int imageIndex = -1;
	public string imageMimeType = "image/png";
	public string imageName = "";
	public Texture2D unityTexture;

	public static int FromMaterial(ModelDocument doc, ModelMaterial material, int imageFormat)
	{
		// First, deduplicate the texture. Yinglet Creator specifically only needs this for the eyes,
		// because for the non-eye materials, the materials are deduplicated instead of the textures.
		for (int i = 0; i < doc.textures.Count; i++)
		{
			if (doc.textures[i].unityTexture == material.unityTexture)
			{
				return i;
			}
		}
		return FromUnityTexture2D(doc, material.unityTexture, material.name, imageFormat);
	}

	public static int FromUnityTexture2D(ModelDocument doc, Texture2D unityTexture, string baseName, int imageFormat)
	{
		// Make a new texture.
		ModelTexture modelTexture = new ModelTexture();
		// Convert the base name to snake_case for the texture name.
		modelTexture.name = System.Text.RegularExpressions.Regex.Replace(PascalToSnake(baseName), "[^a-z0-9]", "_");
		modelTexture.name = modelTexture.name.Replace("__", "_").Replace("mat_", "tex_").Replace("material", "texture");
		modelTexture.name = doc.ReserveUniqueName(modelTexture.name);
		modelTexture.unityTexture = unityTexture;
		if (imageFormat == 0) // PNG
		{
			modelTexture.imageMimeType = "image/png";
			modelTexture.imageName = doc.ReserveUniqueName(modelTexture.name + ".png");
			byte[] pngData = modelTexture.unityTexture.EncodeToPNG();
			modelTexture.bufferViewIndex = ModelBufferView.FromByteArrayIntoDoc(doc, pngData);
		}
		else // JPEG
		{
			modelTexture.imageMimeType = "image/jpeg";
			modelTexture.imageName = doc.ReserveUniqueName(modelTexture.name + ".jpg");
			byte[] jpgData = modelTexture.unityTexture.EncodeToJPG();
			modelTexture.bufferViewIndex = ModelBufferView.FromByteArrayIntoDoc(doc, jpgData);
		}
		int texIndex = doc.textures.Count;
		modelTexture.imageIndex = texIndex; // For simplicity, these will be in sync.
		doc.textures.Add(modelTexture);
		return texIndex;
	}

	private static string PascalToSnake(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		System.Text.StringBuilder result = new System.Text.StringBuilder(value.Length + 5);
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (char.IsUpper(c))
			{
				if (i > 0)
				{
					result.Append('_');
				}
				result.Append(char.ToLowerInvariant(c));
			}
			else
			{
				result.Append(c);
			}
		}
		return result.ToString();
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{");
		if (format == ModelBaseFormat.GLTF)
		{
			json.Append("\"name\":\"" + name + "\"");
			json.Append(",\"source\":");
			json.Append(imageIndex);
		}
		else // G3MF
		{
			json.Append("\"images\":[{\"bufferView\":");
			json.Append(bufferViewIndex);
			json.Append(",\"mimeType\":\"");
			json.Append(imageMimeType);
			json.Append("\",\"name\":\"");
			json.Append(imageName);
			json.Append("\"}]");
			json.Append(",\"name\":\"" + name + "\"");
			json.Append(",\"placeholder\":[0.5,0.5,0.5,1.0]");
			json.Append(",\"size\":[");
			json.Append(unityTexture.width);
			json.Append(",");
			json.Append(unityTexture.height);
			json.Append("]");
		}
		json.Append("}");
		return json.ToString();
	}
}
