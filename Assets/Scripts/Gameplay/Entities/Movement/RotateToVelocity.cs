using UnityEngine;

public class RotateToVelocity : MonoBehaviour
{
	[Header("Y-Axis Rotation (Yaw)")]
	[SerializeField] private float MIN_SPEED = 0.1f;
	[SerializeField] private float YAW_SMOOTH = 10f;

	[Header("XZ-Axis Tilt (Pitch/Roll)")]
	[SerializeField] private float TILT_STRENGTH = 0.02f;
	[SerializeField] private float TILT_SMOOTH = 10f;

	private Rigidbody _rb;
	private Vector3 _lastVelocity;
	private Quaternion _yaw;
	private Quaternion _tilt;

	void Awake()
	{
		_rb = this.GetComponentInParent<Rigidbody>();
		_yaw = transform.rotation;
		_tilt = Quaternion.identity;
		_lastVelocity = _rb.linearVelocity;
	}

	void Update()
	{
		UpdateYaw();
		UpdateTilt();
		transform.rotation = _yaw * _tilt;
	}

	void UpdateYaw()
	{
		Vector3 flatVel = _rb.linearVelocity.WithoutY();
		if (flatVel.sqrMagnitude > MIN_SPEED * MIN_SPEED)
		{
			Quaternion targetYaw = Quaternion.LookRotation(flatVel.normalized, Vector3.up);
			_yaw = _yaw.SmoothTo(targetYaw, YAW_SMOOTH, Time.deltaTime);
		}
	}

	void UpdateTilt()
	{
		Vector3 accel = (_rb.linearVelocity - _lastVelocity) / Time.deltaTime;
		_lastVelocity = _rb.linearVelocity;

		// Convert acceleration into local space of the yaw (so tilt is relative to facing)
		Vector3 localAccel = Quaternion.Inverse(_yaw) * accel;

		// Pitch (X) from forward accel (Z), Roll (Z) from sideways accel (X)
		float pitch = -localAccel.z * TILT_STRENGTH;
		float roll = localAccel.x * TILT_STRENGTH;

		Quaternion targetTilt = Quaternion.Euler(pitch, 0f, roll);
		_tilt = _tilt.SmoothTo(targetTilt, TILT_SMOOTH, Time.deltaTime);
	}
}