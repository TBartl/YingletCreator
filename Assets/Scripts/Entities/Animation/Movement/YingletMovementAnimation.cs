using System.Collections;
using UnityEngine;



public class YingletMovementAnimation : MonoBehaviour
{
	[Header("Horizontal")]
	[SerializeField] float SPEED_THRESHOLD = 0.1f;
	[SerializeField] float WALKING_ANIM_SPEED = 1f;
	[SerializeField] float RUNNING_ANIM_SPEED = 1.35f;
	[SerializeField] Vector2 WALK_TO_RUN_RANGE;

	[Header("Vertical")]
	[SerializeField] AnimationCurve VERTICAL_VELOCITY_TO_RISING_WEIGHT;
	[SerializeField] float MAX_IMPACT_TIME = .5f;
	[SerializeField] AnimationCurve IMPACT_CURVE;
	[SerializeField] float MIN_IMPACT_SPEED = 5.5f;
	[SerializeField] float MAX_IMPACT_SPEED = 7f;

	private Rigidbody _rigidBody;
	private ICharacterCollisionHandling _collisionHandling;
	private ICharacterRoundState _roundState;
	private IYingletAnimationBridge _animation;
	private Coroutine _impactGroundCoroutine;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_rigidBody = this.GetComponentInParent<Rigidbody>();
		_collisionHandling = this.GetComponentInParent<ICharacterCollisionHandling>();
		_roundState = this.GetNullableComponentInParentSafe<ICharacterRoundState>();

		_collisionHandling.OnImpactedGround += OnImpactedGround;
		_animation = this.GetComponent<IYingletAnimationBridge>();
	}

	private void OnDestroy()
	{
		_collisionHandling.OnImpactedGround -= OnImpactedGround;
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

		if (!_collisionHandling.Grounded)
		{
			state = YingletAnimState.Airborne;
			_animation.SetRising(VERTICAL_VELOCITY_TO_RISING_WEIGHT.Evaluate(_rigidBody.linearVelocity.y));
		}
		if (_roundState?.IsAsleep?.Val ?? false)
		{
			state = YingletAnimState.Sleeping;
		}

		_animation.SetAnimState(state);
	}

	private void OnImpactedGround(PhysicsMaterial material, float speed, Vector3 position)
	{
		if (speed < MIN_IMPACT_SPEED) return;

		this.StopAndStartCoroutine(ref _impactGroundCoroutine, ImpactGround(speed));
	}
	IEnumerator ImpactGround(float speed)
	{
		float fallIntensity = Mathf.Clamp01((speed - MIN_IMPACT_SPEED) / (MAX_IMPACT_SPEED - MIN_IMPACT_SPEED));
		for (float t = 0; t < MAX_IMPACT_TIME; t += Time.deltaTime)
		{
			float p = t / MAX_IMPACT_TIME;
			_animation.SetFallImpactWeight(fallIntensity * IMPACT_CURVE.Evaluate(p));
			yield return null;
		}
		_animation.SetFallImpactWeight(0);
	}
}
