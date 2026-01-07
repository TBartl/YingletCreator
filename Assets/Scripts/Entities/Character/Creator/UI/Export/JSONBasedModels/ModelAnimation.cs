using UnityEngine;

/// <summary>
/// Since Yinglet Creator only exports animations for eyes, hard-code this class to an eye material texture offset animation.
/// Also hard-code only a single sampler with one of each type of accessor (times and values).
/// </summary>
public class ModelAnimation : ModelItem
{
	public int targetMaterialIndex = -1;
	public int timesAccessorIndex = -1;
	public int valuesAccessorIndex = -1;
	public Vector2 offset = Vector2.zero;
	private static int _zeroTimeAccessorIndex = -1;

	public static ModelAnimation FromEyeExpression(ModelDocument doc, int materialIndex, Rect anyEyeRect, EyeExpression thisEyeExpression, EyeExpression baseEyeExpression)
	{
		_zeroTimeAccessorIndex = -1; // Reset static data to avoid corruption from the previous export.
		ModelAnimation modelAnimation = new ModelAnimation();
		modelAnimation.targetMaterialIndex = materialIndex;
		int thisCol = (int)thisEyeExpression % 4;
		int thisRow = (int)thisEyeExpression / 4;
		int baseCol = (int)baseEyeExpression % 4;
		int baseRow = (int)baseEyeExpression / 4;
		float offsetX = anyEyeRect.width * 0.25f;
		float offsetY = anyEyeRect.height * 0.5f;
		modelAnimation.offset = new Vector2(offsetX * (thisCol - baseCol), offsetY * (thisRow - baseRow));
		return modelAnimation;
	}

	public void EncodeZeroTimeAnimationAccessors(ModelDocument doc, ModelBaseFormat format)
	{
		if (format == ModelBaseFormat.GLTF)
		{
			// Yinglet Creator specific hack: Only ever encode a times accessor with a single time of zero.
			if (_zeroTimeAccessorIndex == -1)
			{
				float[] zero = new float[] { 0.0f };
				_zeroTimeAccessorIndex = ModelAccessor.EncodeFloats(doc, zero, ModelBufferView.ArrayBufferglTFTarget.NONE);
				doc.accessors[_zeroTimeAccessorIndex].SetMaxAndMinForFloats(zero);
			}
			timesAccessorIndex = _zeroTimeAccessorIndex;
		}
		valuesAccessorIndex = ModelAccessor.EncodeVector2s(doc, new Vector2[] { offset }, ModelBufferView.ArrayBufferglTFTarget.NONE);
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		_zeroTimeAccessorIndex = -1; // Reset static data to avoid corrupting the next export.
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{");
		if (format == ModelBaseFormat.GLTF)
		{
			json.Append("\"channels\":[{");
			json.Append("\"sampler\":0,");
			json.Append("\"target\":{");
			json.Append("\"path\":\"pointer\",");
			json.Append("\"extensions\":{");
			json.Append("\"KHR_animation_pointer\":{");
			json.Append("\"pointer\":\"/materials/");;
			json.Append(targetMaterialIndex);
			json.Append("/pbrMetallicRoughness/baseColorTexture/extensions/KHR_texture_transform/offset\"");;
			json.Append("}"); // End KHR_animation_pointer
			json.Append("}"); // End extensions
			json.Append("}"); // End target
			json.Append("}],"); // End channels
			json.Append("\"samplers\":[{");
			json.Append("\"input\":");
			json.Append(timesAccessorIndex);
			json.Append(",\"interpolation\":\"STEP\"");
			json.Append(",\"output\":");
			json.Append(valuesAccessorIndex);
			json.Append("}],");
		}
		json.Append("\"name\":\"" + name + "\"");
		if (format == ModelBaseFormat.G3MF)
		{
			json.Append(",\"tracks\":[{");
			json.Append("\"interpolation\":\"STEP\"");
			json.Append(",\"target\":\"/materials/");
			json.Append(targetMaterialIndex);
			json.Append("/baseColor/extensions/KHR_texture_transform/offset\"");
			if (timesAccessorIndex != -1)
			{
				json.Append(",\"times\":");
				json.Append(timesAccessorIndex);
			}
			json.Append(",\"values\":");
			json.Append(valuesAccessorIndex);
			json.Append("}]");
		}
		json.Append("}");
		return json.ToString();
	}
}
