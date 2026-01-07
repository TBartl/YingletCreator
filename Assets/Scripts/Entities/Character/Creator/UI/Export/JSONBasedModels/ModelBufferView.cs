public class ModelBufferView : ModelItem
{
	public enum ArrayBufferglTFTarget
	{
		NONE = 0,
		ARRAY_BUFFER = 34962,
		ELEMENT_ARRAY_BUFFER = 34963
	}
	public ArrayBufferglTFTarget gltfTarget = ArrayBufferglTFTarget.NONE;
	/// <summary>
	/// The length in bytes of this buffer view.
	/// </summary>
	public long byteLength = -1;
	/// <summary>
	/// The byte offset within the buffer where this buffer view starts.
	/// </summary>
	public long byteOffset = 0;

	public static int FromByteArrayIntoDoc(ModelDocument doc, byte[] data)
	{
		ModelBufferView bufferView = new ModelBufferView();
		bufferView.byteLength = data.Length;
		bufferView.byteOffset = doc.bufferData.Count;
		doc.bufferData.AddRange(data);
		int bufferViewIndex = doc.bufferViews.Count;
		doc.bufferViews.Add(bufferView);
		return bufferViewIndex;
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		string text = "{";
		if (format == ModelBaseFormat.GLTF)
		{
			text = "{\"buffer\":0,";
		}
		text += "\"byteLength\":" + byteLength;
		if (byteOffset > 0)
		{
			text += ",\"byteOffset\":" + byteOffset;
		}
		if (format == ModelBaseFormat.GLTF && gltfTarget != ArrayBufferglTFTarget.NONE)
		{
			text += ",\"target\":" + (int)gltfTarget;
		}
		return text + "}";
	}
}
