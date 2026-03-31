using UnityEngine;

public interface IAccelerationTracker
{
	/// <summary>
	/// Returns the acceleration on the XZ plane
	/// </summary>
	Vector3 AccelerationXZ { get; }
}

public class AccelerationTracker : MonoBehaviour, IAccelerationTracker
{
	Rigidbody _rb;
	private Vector3 _lastVelocity;

	public Vector3 AccelerationXZ { get; private set; }

	void Awake()
	{
		_rb = this.GetComponent<Rigidbody>();
		_lastVelocity = _rb.linearVelocity;
	}

	void FixedUpdate()
	{
		Vector3 accel = (_rb.linearVelocity - _lastVelocity) / Time.deltaTime;
		_lastVelocity = _rb.linearVelocity;
		AccelerationXZ = accel.WithoutY();
	}
}
