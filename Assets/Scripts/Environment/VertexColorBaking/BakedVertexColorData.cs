using UnityEngine;


[ExecuteInEditMode]
public class BakedVertexColorData : MonoBehaviour
{
	[SerializeField][HideInInspector] Color[] _colors = new Color[0];

	void OnEnable()
	{
		if (_colors.Length == 0) return; // No colors to apply

		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

		if (meshFilter.sharedMesh == null || meshRenderer == null) return;

		// Create a mesh for additional vertex streams with the same vertices and colors
		Mesh additionalStream = new Mesh();
		additionalStream.vertices = meshFilter.sharedMesh.vertices;
		additionalStream.colors = _colors;
		additionalStream.hideFlags = HideFlags.DontSave;

		// Apply the additional vertex stream to the mesh renderer
		meshRenderer.additionalVertexStreams = additionalStream;
	}

	void OnDisable()
	{
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer != null)
		{
			meshRenderer.additionalVertexStreams = null;
		}
	}


#if UNITY_EDITOR
	public void SetColors(Color[] colors)
	{
		_colors = colors;
		OnEnable();
	}
#endif
}