using System.Collections;
using Unity.Netcode;
using UnityEngine;

public delegate void JumpEvent(Vector3 position, Vector3 velocity);

public interface ICharacterMovement
{
	event JumpEvent OnJump;
}

public class CharacterMovement : MonoBehaviour, ICharacterMovement
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

	private INetEventBus _eventBus;
	private IPlayerIdentity _identity;
	private INetworkRigidbody _networkRB;
	private IInputRestrictor _inputRestrictor;
	private Rigidbody _rb;
	private ICharacterCollisionHandling _collisionHandling;
	private float _jumpInputTime = -100;

	public event JumpEvent OnJump = delegate { };

	private void Awake()
	{
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_identity = this.GetComponentInParent<IPlayerIdentity>();
		_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
		_networkRB = this.GetComponent<INetworkRigidbody>();
		_rb = this.GetComponent<Rigidbody>();
		_collisionHandling = this.GetComponent<ICharacterCollisionHandling>();

		_eventBus.Subscribe<Message_Jump>(OnMessageJump);
	}
	private void OnDestroy()
	{
		_eventBus.Unsubscribe<Message_Jump>(OnMessageJump);
	}


	void Update()
	{
		if (!InputAllowed) return;
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			_jumpInputTime = Time.time;
		}
	}

	void FixedUpdate()
	{
		UpdateHorizontal();
		UpdateVertical();
		UpdateOutOfBoundsHandling();
	}

	void UpdateHorizontal()
	{
		if (!_identity.IsMine) return; // Don't use InputAllowed since we still want this to apply to ourselves as friction just in case

		// Figure out the ideal speed
		var targetDirection = Vector3.zero;
		if (_inputRestrictor.InputAllowed && _identity.IsActive)
		{
			targetDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		}

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

		if (!InputAllowed) return;
		if (!_collisionHandling.CanJump) return;

		bool jumpRecentlyPressed = Time.time < _jumpInputTime + JUMP_BUFFER_TIME;
		if (!jumpRecentlyPressed) return;

		Vector3 vel = _rb.linearVelocity;
		vel.y = JUMP_SPEED;
		_rb.linearVelocity = vel;

		OnJump(this.transform.position, vel);
		SendJumpMessage(this.transform.position);
		_collisionHandling.ClearCanJump();
		_jumpInputTime = 0;
	}

	bool UseLowGravity()
	{
		// User not alowed to input anything? Full gravity
		if (!InputAllowed) return false;

		// Passed peak of jump? Full gravity
		if (_rb.linearVelocity.y < LOW_GRAVITY_VELOCITY_PEAK) return false;

		// User not holding jump any more? Full gravity
		if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.JoystickButton0)) return false;

		return true;
	}

	private void UpdateOutOfBoundsHandling()
	{
		if (!_identity.IsMine) return;

		// Just something simple for now
		if (this.transform.position.y < -50)
		{
			var root = this.transform.position.x < -100 ? Vector3.left * 200 : Vector3.zero;
			_rb.MovePosition(root + Vector3.up * 3);
		}
	}

	Vector3 ClampMagnitude1(Vector3 v)
	{
		return v.sqrMagnitude > 1f ? v.normalized : v;
	}

	bool InputAllowed
	{
		get
		{
			if (!_identity.IsActive) return false;
			if (!_inputRestrictor.InputAllowed) return false;
			return true;
		}
	}

	void SendJumpMessage(Vector3 position)
	{
		_eventBus.SendToAll(new Message_Jump
		{
			Position = position,
			Velocity = _rb.linearVelocity
		});
	}

	private void OnMessageJump(Message_Jump message, ulong senderClientId)
	{
		if (senderClientId != _identity.ConnectionId) return;
		if (_identity.IsMine) return;
		StartCoroutine(DelayJump(message.Position, message.Velocity));
	}
	IEnumerator DelayJump(Vector3 position, Vector3 velocity)
	{
		yield return new WaitForSeconds((float)_networkRB.BufferTime);
		OnJump(position, velocity);
	}
}

public struct Message_Jump : INetMessage
{
	public Vector3 Position;
	public Vector3 Velocity;
	public NetworkDelivery DeliveryMethod => NetworkDelivery.Reliable;
	public bool SendToSelf => false;
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref Position);
		serializer.SerializeValue(ref Velocity);
	}
}