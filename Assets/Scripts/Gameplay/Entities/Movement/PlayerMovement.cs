using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float _maxSpeed = 3;
	[SerializeField] float _maxWalkSpeed = 1.5f;
	[SerializeField] float _acceleration;
	[SerializeField] AnimationCurve _accelMultiplierByNormalizedVelocityDist;
	private Rigidbody _rb;

	private void Awake()
	{
		_rb = this.GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		UpdateHorizontal();
		UpdateVertical();
	}


	void UpdateHorizontal()
	{

		// Figure out the ideal speed
		var targetDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		targetDirection = ClampMagnitude1(targetDirection); // Don't exceed 1

		float maxSpeed = Input.GetKey(KeyCode.LeftShift) ? _maxWalkSpeed : _maxSpeed;

		var targetVelocity = targetDirection * maxSpeed;

		var currentVelocity = _rb.linearVelocity.WithoutY();

		// Get the difference between the current velocity and the ideal velocity
		var velocityDifference = targetVelocity - currentVelocity;

		// Get the difference between the current velocity and the ideal velocity
		var normalizedVelocityDist = Mathf.Abs(Vector3.Distance(targetVelocity, currentVelocity)) / _maxSpeed;
		var accelMultiplier = _accelMultiplierByNormalizedVelocityDist.Evaluate(normalizedVelocityDist);
		var acceleration = _acceleration * accelMultiplier;

		// Apply acceleration in the best way to get us from the current velocity to the ideal velocity
		var accToApply = Mathf.Min(velocityDifference.magnitude / Time.fixedDeltaTime, acceleration);
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
