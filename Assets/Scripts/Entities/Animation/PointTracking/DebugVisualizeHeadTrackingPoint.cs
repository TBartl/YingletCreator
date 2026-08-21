using UnityEngine;

public class DebugVisualizeHeadTrackingPoint : MonoBehaviour
{
	private IPointTrackingLocationProvider _pointLocationProvider;
	private MeshRenderer _mr;

	void Start()
	{
		_pointLocationProvider = FindAnyObjectByType<CharacterCreatorPointTrackingLocationProvider>();
		_mr = this.GetComponent<MeshRenderer>();
	}

	void LateUpdate()
	{
		this.transform.position = _pointLocationProvider.Position;
		_mr.enabled = _pointLocationProvider.Active.Val;
	}
}
