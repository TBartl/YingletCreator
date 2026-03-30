using UnityEngine;

public class RotateToVelocity : MonoBehaviour
{
	[SerializeField] float _speedThreshold = 0.1f;
	[SerializeField] float _rotationSpeed = 10f;

	private Rigidbody _rb;

	void Start()
	{
		_rb = GetComponentInParent<Rigidbody>();
	}

	void LateUpdate()
	{
		Vector3 horizontalVelocity = _rb.linearVelocity.WithoutY();
		float speed = horizontalVelocity.magnitude;

		if (speed < _speedThreshold) return;

		Vector3 moveDirection = horizontalVelocity.normalized;

		Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up) * Quaternion.Euler(0, 180, 0);

		// Smoothly rotate to face the movement direction
		transform.rotation = Quaternion.Lerp(
			transform.rotation,
			targetRotation,
			_rotationSpeed * Time.deltaTime
		);
	}
}
