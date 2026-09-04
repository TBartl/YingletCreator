using UnityEngine;
using UnityEngine.Serialization;

public class RotatePivotTowardsCamera : MonoBehaviour
{
	const float CHILD_RADIUS = .2f;

	private Camera _camera;

	[FormerlySerializedAs("_realAngleToRestrictedAngle")]
	[SerializeField] AnimationCurve _cameraTiltToRestrictedAngle;

	void Start()
	{
		_camera = Camera.main;
	}

	void LateUpdate()
	{
		if (_camera == null) return;

		Vector3 forward = _camera.transform.forward;
		float tilt = Mathf.Atan2(
			-forward.y,
			new Vector2(forward.x, forward.z).magnitude
		) * Mathf.Rad2Deg;

		tilt = _cameraTiltToRestrictedAngle.Evaluate(tilt);

		// Apply rotation around X-axis
		transform.localRotation = Quaternion.Euler(tilt, 0, 0);

		// Shift the sprite along Z to maintain its center position
		// As the pivot rotates, the sprite's center moves, so we need to compensate
		float zOffset = CHILD_RADIUS * Mathf.Sin(tilt * Mathf.Deg2Rad);
		transform.localPosition = new Vector3(0, 0, -zOffset);
	}
}
