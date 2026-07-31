using UnityEngine;

public class RoomMeshVisibilityConstants : MonoBehaviour, IInitializable
{
	[field: SerializeField] public SharedEaseSettings EaseSettings { get; private set; }
	[field: SerializeField] public Vector2 PassageRange { get; private set; }

	public int Y_CUTOFF_PROPERTY_ID { get; private set; }
	public Shader CUTOFF_FROM_TOP_SHADER { get; private set; }

	public void Initialize()
	{
		Y_CUTOFF_PROPERTY_ID = Shader.PropertyToID("_YCutoff");
		CUTOFF_FROM_TOP_SHADER = Shader.Find("Shader Graphs/CutOffFromTop");
	}
}
