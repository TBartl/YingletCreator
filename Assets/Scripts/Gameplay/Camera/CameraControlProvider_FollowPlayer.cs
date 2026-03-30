

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FollowPlayer : ReactiveBehaviour, ICameraControlProvider
{
	[SerializeField] Rigidbody _target;

	[SerializeField] float OFFSET_BACK = 5;
	[SerializeField] float OFFSET_UP = 3;
	[SerializeField] float OFFSET_PIVOT_UP = 2;
	[SerializeField] float VELOCITY_OFFSET_MULTIPLIER = 0.5f;
	[SerializeField] float LERPING_POWER = 1;

	private Vector3 _pos;
	private Quaternion _rot;

	public bool WantsControl => true; // This is effectively the default for now (unless player despawns or something idk we'll figure that out)

	void LateUpdate()
	{
		if (_target == null) return;

		var lastPos = _pos;

		var pivotPoint = _target.position + Vector3.up * OFFSET_PIVOT_UP;
		pivotPoint += _target.linearVelocity.WithoutY() * VELOCITY_OFFSET_MULTIPLIER; // Look ahead in the direction we're moving

		var targetPos = pivotPoint + Vector3.back * OFFSET_BACK + Vector3.up * OFFSET_UP;
		_pos = Vector3.Lerp(lastPos, targetPos, LERPING_POWER * Time.deltaTime);
		_rot = Quaternion.LookRotation(pivotPoint - targetPos, Vector3.up);
	}

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (_pos, _rot);
	}
}
