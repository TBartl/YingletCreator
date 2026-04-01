using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Horizontal")]
	[SerializeField] float MAX_RUN_SPEED = 3.8f;
	[SerializeField] float MAX_WALK_SPEED = 0.9f;
	[SerializeField] float RUN_ACCELERATION = 6.8f;
	[SerializeField] AnimationCurve ACCEL_MULTIPLIER_BY_NORMALIZED_VELOCITY_DIST;

	[Header("Vertical")]
	[SerializeField] float JUMP_SPEED = 5;
	[SerializeField] float JUMP_BUFFER_TIME = .15f;
	[SerializeField] float GRAVITY = 9.81f;
	[SerializeField] float LOW_GRAVITY_MULTIPLIER = 0.5f;
	[SerializeField] float LOW_GRAVITY_VELOCITY_PEAK = 7.73f;

	private Rigidbody _rb;
	private IPlayerCollisionHandling _collisionHandling;
	private float _jumpInputTime = -100;

	private void Awake()
	{
		_rb = this.GetComponent<Rigidbody>();
		_collisionHandling = this.GetComponent<IPlayerCollisionHandling>();
	}
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			_jumpInputTime = Time.time;
		}
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

		float maxSpeed = Input.GetKey(KeyCode.LeftShift) ? MAX_WALK_SPEED : MAX_RUN_SPEED;

		var targetVelocity = targetDirection * maxSpeed;

		var currentVelocity = _rb.linearVelocity.WithoutY();

		// Get the difference between the current velocity and the ideal velocity
		var velocityDifference = targetVelocity - currentVelocity;

		// Get the difference between the current velocity and the ideal velocity
		var normalizedVelocityDist = Mathf.Abs(Vector3.Distance(targetVelocity, currentVelocity)) / MAX_RUN_SPEED;
		var accelMultiplier = ACCEL_MULTIPLIER_BY_NORMALIZED_VELOCITY_DIST.Evaluate(normalizedVelocityDist);
		var acceleration = RUN_ACCELERATION * accelMultiplier;

		// Apply acceleration in the best way to get us from the current velocity to the ideal velocity
		var accToApply = Mathf.Min(velocityDifference.magnitude / Time.fixedDeltaTime, acceleration);
		_rb.AddForce(velocityDifference.normalized * accToApply, ForceMode.Acceleration);
	}

	void UpdateVertical()
	{
		float gravity = GRAVITY;
		if (UseLowGravity())
		{
			gravity *= LOW_GRAVITY_MULTIPLIER;
		}
		_rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

		if (!_collisionHandling.CanJump) return;

		bool jumpRecentlyPressed = Time.time < _jumpInputTime + JUMP_BUFFER_TIME;
		if (!jumpRecentlyPressed) return;

		Vector3 vel = _rb.linearVelocity;
		vel.y = JUMP_SPEED;
		_rb.linearVelocity = vel;

		_collisionHandling.ClearCanJump();
		_jumpInputTime = 0;
	}

	bool UseLowGravity()
	{
		// User not holding jump any more? Full gravity
		if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.JoystickButton0)) return false;

		// Passed peak of jump? Full gravity
		if (_rb.linearVelocity.y < LOW_GRAVITY_VELOCITY_PEAK) return false;

		return true;
	}
	Vector3 ClampMagnitude1(Vector3 v)
	{
		return v.sqrMagnitude > 1f ? v.normalized : v;
	}
}
