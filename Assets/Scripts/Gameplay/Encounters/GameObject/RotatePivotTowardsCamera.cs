using UnityEngine;

public class RotatePivotTowardsCamera : MonoBehaviour
{
	const float CHILD_RADIUS = .2f;

	private Camera _camera;

	[SerializeField] AnimationCurve _realAngleToRestrictedAngle;

	void Start()
	{
		_camera = Camera.main;
	}

	void LateUpdate()
	{
		if (_camera == null) return;

		// Calculate direction from pivot to camera
		Vector3 directionToCamera = _camera.transform.position - transform.position;

		// Calculate the angle we need to rotate around X-axis to face the camera
		// We only care about the vertical component (Y) and depth (Z)
		float verticalDistance = directionToCamera.y;
		float depthDistance = directionToCamera.z;
		float angleX = Mathf.Atan2(verticalDistance, -depthDistance) * Mathf.Rad2Deg;

		angleX = _realAngleToRestrictedAngle.Evaluate(angleX);

		// Apply rotation around X-axis
		transform.localRotation = Quaternion.Euler(angleX, 0, 0);

		// Shift the sprite along Z to maintain its center position
		// As the pivot rotates, the sprite's center moves, so we need to compensate
		float zOffset = CHILD_RADIUS * Mathf.Sin(angleX * Mathf.Deg2Rad);
		transform.localPosition = new Vector3(0, 0, -zOffset);
	}
}
