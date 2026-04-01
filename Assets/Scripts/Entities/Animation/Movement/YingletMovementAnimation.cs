using UnityEngine;



public class YingletMovementAnimation : MonoBehaviour
{
	[Header("Horizontal")]
	[SerializeField] float SPEED_THRESHOLD = 0.1f;
	[SerializeField] float WALKING_ANIM_SPEED = 1f;
	[SerializeField] float RUNNING_ANIM_SPEED = 1.35f;
	[SerializeField] Vector2 WALK_TO_RUN_RANGE;

	[Header("Vertical")]


	private Rigidbody _rigidBody;
	private IPlayerCollisionHandling _collisionHandling;
	private IYingletAnimationBridge _animation;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_rigidBody = this.GetComponentInParent<Rigidbody>();
		_collisionHandling = this.GetComponentInParent<IPlayerCollisionHandling>();
		_animation = this.GetComponent<IYingletAnimationBridge>();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		var state = YingletAnimState.Idle;

		float horizontalSpeed = _rigidBody.linearVelocity.WithoutY().magnitude;
		bool moving = horizontalSpeed > SPEED_THRESHOLD;

		if (moving)
		{
			state = YingletAnimState.Moving;
			float moveType = Mathf.Lerp(0, 1, (horizontalSpeed - WALK_TO_RUN_RANGE.x) / (WALK_TO_RUN_RANGE.y - WALK_TO_RUN_RANGE.x));
			float animSpeed = Mathf.Lerp(WALKING_ANIM_SPEED, RUNNING_ANIM_SPEED, moveType);
			_animation.SetMoveCycleSpeed(horizontalSpeed * animSpeed);
			_animation.SetMoveType(moveType);
		}

		Debug.Log(_collisionHandling.Grounded);
		if (!_collisionHandling.Grounded)
		{
			state = YingletAnimState.Airborne;
		}
		_animation.SetAnimState(state);
	}
}
