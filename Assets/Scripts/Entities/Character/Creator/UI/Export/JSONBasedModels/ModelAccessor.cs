using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ModelAccessor : ModelItem
{
	/// <summary>
	/// The index of the buffer view containing the data for this accessor.
	/// </summary>
	public int bufferViewIndex = -1;
	/// <summary>
	/// The number of elements (vectors) in this accessor. Redundant with buffer view length.
	/// </summary>
	public long count = -1;
	/// <summary>
	/// The component data type of each primitive number stored in this accessor.
	/// </summary>
	public string componentType = "float32";
	/// <summary>
	/// The number of numbers per vector (scalar, Vector2, Vector3, etc).
	/// </summary>
	public int vectorSize = 1;

	// Min and max values for the accessor data, only used for glTF export.
	private float[] _max;
	private float[] _min;

	// 255 and 65535 are reserved as primitive restart values in glTF.
	private const int _MAX_UINT8 = 254;
	private const int _MAX_UINT16 = 65534;
	public static string preferredFloatComponentType = "float32";

	public static int EncodeUIntsFromInt32s(ModelDocument doc, ICollection<int> numbers, ModelBufferView.ArrayBufferglTFTarget gltfTarget = ModelBufferView.ArrayBufferglTFTarget.NONE)
	{
		ModelAccessor accessor = new ModelAccessor();
		accessor.count = numbers.Count;
		accessor.componentType = DetermineMinimalUIntComponentType(numbers);
		accessor.vectorSize = 1;
		// Create buffer view.
		int bytesPerPrim = accessor.GetBytesPerComponent();
		int padding = (bytesPerPrim - (doc.bufferData.Count % bytesPerPrim)) % bytesPerPrim;
		byte[] bytes = accessor.CreateByteArrayForBufferViewData(doc, numbers.Count, padding, gltfTarget);
		// Copy vector bytes into the destination after padding.
		int dst = 0;
		Span<byte> dstBytes = bytes.AsSpan(padding);
		foreach (int num in numbers)
		{
			accessor.WriteUIntToByteArray(dstBytes, ref dst, num);
		}
		doc.bufferData.AddRange(bytes);
		int accessorIndex = doc.accessors.Count;
		doc.accessors.Add(accessor);
		return accessorIndex;
	}

	public static int EncodeFloats(ModelDocument doc, ICollection<float> numbers, ModelBufferView.ArrayBufferglTFTarget gltfTarget = ModelBufferView.ArrayBufferglTFTarget.NONE)
	{
		ModelAccessor accessor = new ModelAccessor();
		accessor.count = numbers.Count;
		accessor.componentType = preferredFloatComponentType;
		accessor.vectorSize = 1;
		// Create buffer view.
		int bytesPerPrim = accessor.GetBytesPerComponent();
		int padding = (bytesPerPrim - (doc.bufferData.Count % bytesPerPrim)) % bytesPerPrim;
		byte[] bytes = accessor.CreateByteArrayForBufferViewData(doc, numbers.Count, padding, gltfTarget);
		// Copy vector bytes into the destination after padding.
		int dst = 0;
		Span<byte> dstBytes = bytes.AsSpan(padding);
		foreach (float number in numbers)
		{
			accessor.WriteFloatToByteArray(dstBytes, ref dst, number);
		}
		doc.bufferData.AddRange(bytes);
		int accessorIndex = doc.accessors.Count;
		doc.accessors.Add(accessor);
		return accessorIndex;
	}

	public static int EncodeVector2s(ModelDocument doc, ICollection<Vector2> vectors, ModelBufferView.ArrayBufferglTFTarget gltfTarget = ModelBufferView.ArrayBufferglTFTarget.NONE)
	{
		ModelAccessor accessor = new ModelAccessor();
		accessor.count = vectors.Count;
		accessor.componentType = preferredFloatComponentType;
		accessor.vectorSize = 2;
		// Create buffer view.
		int bytesPerPrim = accessor.GetBytesPerComponent();
		int padding = (bytesPerPrim - (doc.bufferData.Count % bytesPerPrim)) % bytesPerPrim;
		byte[] bytes = accessor.CreateByteArrayForBufferViewData(doc, vectors.Count, padding, gltfTarget);
		// Copy vector bytes into the destination after padding.
		int dst = 0;
		Span<byte> dstBytes = bytes.AsSpan(padding);
		foreach (Vector2 v in vectors)
		{
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.x);
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.y);
		}
		doc.bufferData.AddRange(bytes);
		int accessorIndex = doc.accessors.Count;
		doc.accessors.Add(accessor);
		return accessorIndex;
	}

	// Returns IEEE 754 binary16 bits (sign:1, exponent:5, mantissa:10).
	// Note: This is not needed for .NET 5+ which includes System.Half, but Unity is still on an older version.
	public static ushort Float32ToFloat16(float value)
	{
		uint bits = (uint)BitConverter.SingleToInt32Bits(value);
		uint f32Exponent = (bits >> 23) & 0xFFu;
		uint f32Mantissa = bits & ((1u << 23) - 1u);
		ushort f16Sign = (ushort)((bits >> 16) & 0x8000u);
		if (f32Exponent == 0xFFu)
		{
			// Infinity or NaN.
			ushort f16Mantissa = (ushort)(f32Mantissa >> 13); // Top 10 bits.
															  // Preserve NaN as NaN (don't let it become infinity).
			if (f16Mantissa == 0 && f32Mantissa != 0)
			{
				f16Mantissa = 0x1;
			}
			return (ushort)(f16Sign | (ushort)(0x1F << 10) | f16Mantissa);
		}
		int f32RealExp = (int)f32Exponent - 127;
		if (f32RealExp > 15)
		{
			// Overflow: too large, becomes Infinity.
			return (ushort)(f16Sign | (ushort)(0x1F << 10));
		}
		if (f32RealExp < -14)
		{
			// Subnormal or underflow to zero.
			if (f32RealExp < -25)
			{
				// Too small -> flush to zero.
				return f16Sign;
			}
			// Convert to subnormal.
			int shift = -14 - f32RealExp;
			uint f32MantissaBits = (1u << 23) | f32Mantissa;
			uint roundBit = (f32MantissaBits >> (23 + shift - 11)) & 1u;
			// 10-bit mantissa.
			uint f16MantissaBits = f32MantissaBits >> (23 + shift - 10);
			// Round and clamp.
			f16MantissaBits += roundBit;
			if (f16MantissaBits > 0x3FFu)
			{
				f16MantissaBits = 0x3FFu;
			}
			return (ushort)(f16Sign | (ushort)f16MantissaBits);
		}
		// Normal case.
		uint normalRoundBit = (f32Mantissa >> (23 - 11)) & 1u;
		ushort f16Exponent = (ushort)(f32RealExp + 15); // Re-bias to float16.
		uint f16MantissaData = f32Mantissa >> (23 - 10);
		f16MantissaData += normalRoundBit;
		// If mantissa overflows, increment exponent.
		if (f16MantissaData > 0x3FFu)
		{
			f16MantissaData = 0;
			f16Exponent = (ushort)(f16Exponent + 1);

			if (f16Exponent >= 0x1F)
			{
				return (ushort)(f16Sign | (ushort)(0x1F << 10));
			}
		}
		return (ushort)(f16Sign | (ushort)(f16Exponent << 10) | (ushort)f16MantissaData);
	}

	public static int EncodeVector3s(ModelDocument doc, ICollection<Vector3> vectors, ModelBufferView.ArrayBufferglTFTarget gltfTarget = ModelBufferView.ArrayBufferglTFTarget.NONE)
	{
		ModelAccessor accessor = new ModelAccessor();
		accessor.componentType = preferredFloatComponentType;
		accessor.vectorSize = 3;
		accessor.count = vectors.Count;
		// Create buffer view.
		int bytesPerPrim = accessor.GetBytesPerComponent();
		int padding = (bytesPerPrim - (doc.bufferData.Count % bytesPerPrim)) % bytesPerPrim;
		byte[] bytes = accessor.CreateByteArrayForBufferViewData(doc, vectors.Count, padding, gltfTarget);
		// Copy vector bytes into the destination after padding.
		int dst = 0;
		Span<byte> dstBytes = bytes.AsSpan(padding);
		foreach (Vector3 v in vectors)
		{
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.x);
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.y);
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.z);
		}
		doc.bufferData.AddRange(bytes);
		int accessorIndex = doc.accessors.Count;
		doc.accessors.Add(accessor);
		return accessorIndex;
	}

	public static int EncodeVector4s(ModelDocument doc, ICollection<Vector4> vectors, ModelBufferView.ArrayBufferglTFTarget gltfTarget = ModelBufferView.ArrayBufferglTFTarget.NONE)
	{
		ModelAccessor accessor = new ModelAccessor();
		accessor.componentType = preferredFloatComponentType;
		accessor.vectorSize = 4;
		accessor.count = vectors.Count;
		// Create buffer view.
		int bytesPerPrim = accessor.GetBytesPerComponent();
		int padding = (bytesPerPrim - (doc.bufferData.Count % bytesPerPrim)) % bytesPerPrim;
		byte[] bytes = accessor.CreateByteArrayForBufferViewData(doc, vectors.Count, padding, gltfTarget);
		// Copy vector bytes into the destination after padding.
		int dst = 0;
		Span<byte> dstBytes = bytes.AsSpan(padding);
		foreach (Vector4 v in vectors)
		{
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.x);
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.y);
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.z);
			accessor.WriteFloatToByteArray(dstBytes, ref dst, v.w);
		}
		doc.bufferData.AddRange(bytes);
		int accessorIndex = doc.accessors.Count;
		doc.accessors.Add(accessor);
		return accessorIndex;
	}

	public static int EncodeUIntsFromVector4Ints(ModelDocument doc, ICollection<Vector4Int> vectors, ModelBufferView.ArrayBufferglTFTarget gltfTarget = ModelBufferView.ArrayBufferglTFTarget.NONE)
	{
		ModelAccessor accessor = new ModelAccessor();
		accessor.componentType = DetermineMinimalUIntComponentType(vectors);
		accessor.vectorSize = 4;
		accessor.count = vectors.Count;
		// Create buffer view.
		int bytesPerPrim = accessor.GetBytesPerComponent();
		int padding = (bytesPerPrim - (doc.bufferData.Count % bytesPerPrim)) % bytesPerPrim;
		byte[] bytes = accessor.CreateByteArrayForBufferViewData(doc, vectors.Count, padding, gltfTarget);
		// Copy vector bytes into the destination after padding.
		int dst = 0;
		Span<byte> dstBytes = bytes.AsSpan(padding);
		foreach (Vector4Int v in vectors)
		{
			accessor.WriteUIntToByteArray(dstBytes, ref dst, v.x);
			accessor.WriteUIntToByteArray(dstBytes, ref dst, v.y);
			accessor.WriteUIntToByteArray(dstBytes, ref dst, v.z);
			accessor.WriteUIntToByteArray(dstBytes, ref dst, v.w);
		}
		doc.bufferData.AddRange(bytes);
		int accessorIndex = doc.accessors.Count;
		doc.accessors.Add(accessor);
		return accessorIndex;
	}

	public static int EncodeMatrix4x4s(ModelDocument doc, ICollection<Matrix4x4> matrices)
	{
		ModelAccessor accessor = new ModelAccessor();
		accessor.componentType = preferredFloatComponentType;
		accessor.vectorSize = 16;
		accessor.count = matrices.Count;
		// Create buffer view.
		int bytesPerPrim = accessor.GetBytesPerComponent();
		int padding = (bytesPerPrim - (doc.bufferData.Count % bytesPerPrim)) % bytesPerPrim;
		byte[] bytes = accessor.CreateByteArrayForBufferViewData(doc, matrices.Count, padding, ModelBufferView.ArrayBufferglTFTarget.NONE);
		// Copy vector bytes into the destination after padding.
		int dst = 0;
		Span<byte> dstBytes = bytes.AsSpan(padding);
		foreach (Matrix4x4 m in matrices)
		{
			for (int i = 0; i < 16; i++)
			{
				accessor.WriteFloatToByteArray(dstBytes, ref dst, m[i]);
			}
		}
		doc.bufferData.AddRange(bytes);
		int accessorIndex = doc.accessors.Count;
		doc.accessors.Add(accessor);
		return accessorIndex;
	}

	private void WriteFloatToByteArray(Span<byte> dstBytes, ref int dst, float num)
	{
		switch (componentType)
		{
			case "float16":
				ushort halfNum = Float32ToFloat16(num);
				MemoryMarshal.Write(dstBytes.Slice(dst, 2), ref halfNum);
				dst += 2;
				break;
			case "float32":
				MemoryMarshal.Write(dstBytes.Slice(dst, 4), ref num);
				dst += 4;
				break;
			default:
				Debug.LogError("Unsupported component type for float: " + componentType);
				break;
		}
	}

	private void WriteUIntToByteArray(Span<byte> dstBytes, ref int dst, int num)
	{
		if (num < 0)
		{
			Debug.LogError("Cannot encode negative number as unsigned integer: " + num);
		}
		switch (componentType)
		{
			case "uint8":
				dstBytes[dst] = (byte)num;
				dst += 1;
				break;
			case "uint16":
				ushort us = (ushort)num;
				MemoryMarshal.Write(dstBytes.Slice(dst, 2), ref us);
				dst += 2;
				break;
			case "uint32":
				uint ui = (uint)num;
				MemoryMarshal.Write(dstBytes.Slice(dst, 4), ref ui);
				dst += 4;
				break;
		}
	}

	public void SetMaxAndMinForFloats(ICollection<float> numbers)
	{
		_max = new float[1] { float.MinValue };
		_min = new float[1] { float.MaxValue };
		foreach (float n in numbers)
		{
			if (n > _max[0]) _max[0] = n;
			if (n < _min[0]) _min[0] = n;
		}
	}

	public void SetMaxAndMinForVector3s(ICollection<Vector3> vectors)
	{
		_max = new float[3] { float.MinValue, float.MinValue, float.MinValue };
		_min = new float[3] { float.MaxValue, float.MaxValue, float.MaxValue };
		foreach (Vector3 v in vectors)
		{
			if (v.x > _max[0]) _max[0] = v.x;
			if (v.y > _max[1]) _max[1] = v.y;
			if (v.z > _max[2]) _max[2] = v.z;
			if (v.x < _min[0]) _min[0] = v.x;
			if (v.y < _min[1]) _min[1] = v.y;
			if (v.z < _min[2]) _min[2] = v.z;
		}
	}

	private byte[] CreateByteArrayForBufferViewData(ModelDocument doc, int count, int padding, ModelBufferView.ArrayBufferglTFTarget gltfTarget)
	{
		ModelBufferView bufferView = new ModelBufferView();
		bufferView.gltfTarget = gltfTarget;
		bufferView.byteLength = GetBytesPerVector() * count;
		bufferViewIndex = doc.bufferViews.Count;
		doc.bufferViews.Add(bufferView);
		bufferView.byteOffset = doc.bufferData.Count + padding;
		return new byte[padding + bufferView.byteLength];
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{\"bufferView\":" + bufferViewIndex);
		if (format == ModelBaseFormat.GLTF)
		{
			json.AppendFormat(",\"componentType\":{0}", ComponentTypeToOpenGL());
			if (count <= 0)
			{
				Debug.LogError("Accessor count is invalid: " + count);
			}
			json.AppendFormat(",\"count\":{0}", count);
			if (_max != null && _min != null)
			{
				// Note: Hard-coded for Scalar and Vector3 because that's what's needed for now, but could be changed later.
				if (_max.Length == 1 && _min.Length == 1)
				{
					json.Append(",\"max\":[" + Flt(_max[0], true) + "]");
					json.Append(",\"min\":[" + Flt(_min[0], true) + "]");
				}
				else if (_max.Length == 3 && _min.Length == 3)
				{
					json.Append(",\"max\":[" + Flt(_max[0], true) + "," + Flt(_max[1], true) + "," + Flt(_max[2], true) + "]");
					json.Append(",\"min\":[" + Flt(_min[0], true) + "," + Flt(_min[1], true) + "," + Flt(_min[2], true) + "]");
				}
			}
			json.AppendFormat(",\"type\":\"{0}\"", VectorSizeToOpenGL());
		}
		else // G3MF
		{
			json.AppendFormat(",\"componentType\":\"{0}\"", componentType);
			if (vectorSize != 1)
			{
				json.AppendFormat(",\"vectorSize\":{0}", vectorSize);
			}
		}
		json.Append("}");
		return json.ToString();
	}

	public int GetBytesPerComponent()
	{
		switch (componentType)
		{
			case "float16": return 2;
			case "float32": return 4;
			case "uint8": return 1;
			case "uint16": return 2;
			case "uint32": return 4;
		}
		Debug.LogError("Unsupported component type: " + componentType);
		return -1;
	}

	public int GetBytesPerVector()
	{
		return GetBytesPerComponent() * vectorSize;
	}

	private string ComponentTypeToOpenGL()
	{
		switch (componentType)
		{
			case "float16": return "5131";
			case "float32": return "5126";
			case "uint8": return "5121";
			case "uint16": return "5123";
			case "uint32": return "5125";
		}
		Debug.LogError("Unsupported component type: " + componentType);
		return "5126";
	}

	private string VectorSizeToOpenGL()
	{
		switch (vectorSize)
		{
			case 1: return "SCALAR";
			case 2: return "VEC2";
			case 3: return "VEC3";
			case 4: return "VEC4";
			case 16: return "MAT4";
		}
		Debug.LogError("Unsupported vector size: " + vectorSize);
		return "SCALAR";
	}

	private static string DetermineMinimalUIntComponentType(IEnumerable<int> numbers)
	{
		string componentType = "uint8";
		foreach (int n in numbers)
		{
			if (componentType == "uint8" && n > _MAX_UINT8)
			{
				componentType = "uint16";
			}
			if (componentType == "uint16" && n > _MAX_UINT16)
			{
				componentType = "uint32";
				break;
			}
		}
		return componentType;
	}

	private static string DetermineMinimalUIntComponentType(IEnumerable<Vector4Int> vectors)
	{
		string componentType = "uint8";
		foreach (Vector4Int v in vectors)
		{
			if (componentType == "uint8")
			{
				if (v.x > _MAX_UINT8 || v.y > _MAX_UINT8 || v.z > _MAX_UINT8 || v.w > _MAX_UINT8)
				{
					componentType = "uint16";
				}
			}
			if (componentType == "uint16")
			{
				if (v.x > _MAX_UINT16 || v.y > _MAX_UINT16 || v.z > _MAX_UINT16 || v.w > _MAX_UINT16)
				{
					componentType = "uint32";
					break;
				}
			}
		}
		return componentType;
	}
}
