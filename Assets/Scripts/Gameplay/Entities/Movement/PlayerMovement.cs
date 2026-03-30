using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float _maxTargetSpeed;
	[SerializeField] float _acceleration;
	private Rigidbody _rb;

	private void Awake()
	{
		_rb = this.GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
	}


	void UpdateHorizontal()
	{

		// Figure out the ideal speed
		var targetDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		targetDirection = ClampMagnitude1(targetDirection); // Don't exceed 1
		var targetVelocity = targetDirection * _maxTargetSpeed;

		var currentHorizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

		// Get the difference between the current velocity and the ideal velocity
		var velocityDifference = targetVelocity - currentHorizontalVelocity;

		// Apply acceleration in the best way to get us from the current velocity to the ideal velocity
		var accToApply = Mathf.Min(velocityDifference.magnitude / Time.fixedDeltaTime, _acceleration);
		_rb.AddForce(velocityDifference.normalized * accToApply, ForceMode.Acceleration);
	}

	void UpdateVertical()
	{
		// TODO
	}
	Vector3 ClampMagnitude1(Vector3 v)
	{
		return v.sqrMagnitude > 1f ? v.normalized : v;
	}
}
