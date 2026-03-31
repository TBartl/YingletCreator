using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class YingletMovementAnimation : MonoBehaviour
{
	[SerializeField] float SPEED_THRESHOLD = 0.1f;
	[SerializeField] float WALKING_ANIM_SPEED = 1f;
	[SerializeField] float RUNNING_ANIM_SPEED = 1.35f;
	[SerializeField] float IDLE_TO_MOVE_BLEND_TIME = 0.25f;
	[SerializeField] Vector2 WALK_TO_RUN_RANGE;

	static string[] IDLE_LAYER_NAMES = new string[] { "TailWagging", "LookAround", "EarWiggle" };
	static string MOVING_LAYER_NAME = "Moving";
	static string MOVE_CYCLE_SPEED_PARAM_NAME = "MoveCycleSpeed";
	static string MOVE_TYPE_PARAM_NAME = "MoveType";

	private Rigidbody _rigidBody;
	private Animator _animator;

	private IEnumerable<YingLayer> _idleLayers;
	private YingLayer _movingLayer;
	private int _moveCycleSpeedParam;
	private int _moveTypeParam;

	float _timeMoving = 0;
	float _timeIdle = 0;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_rigidBody = this.GetComponentInParent<Rigidbody>();
		_animator = this.GetComponent<Animator>();
		_idleLayers = IDLE_LAYER_NAMES.Select(layerName => new YingLayer(layerName, _animator)).ToArray();
		_movingLayer = new YingLayer(MOVING_LAYER_NAME, _animator);
		_moveCycleSpeedParam = Animator.StringToHash(MOVE_CYCLE_SPEED_PARAM_NAME);
		_moveTypeParam = Animator.StringToHash(MOVE_TYPE_PARAM_NAME);
	}

	// Update is called once per frame
	void LateUpdate()
	{

		float speed = _rigidBody.linearVelocity.magnitude;
		bool moving = speed > SPEED_THRESHOLD;

		var idleWeight = UpdateAndGetIdleWeight(moving);

		foreach (var layer in _idleLayers)
		{
			_animator.SetLayerWeight(layer.LayerIndex, idleWeight * layer.OriginalWeight);
		}
		_animator.SetLayerWeight(_movingLayer.LayerIndex, (1 - idleWeight) * _movingLayer.OriginalWeight);
		if (moving)
		{
			float moveType = Mathf.Lerp(0, 1, (speed - WALK_TO_RUN_RANGE.x) / (WALK_TO_RUN_RANGE.y - WALK_TO_RUN_RANGE.x));
			float animSpeed = Mathf.Lerp(WALKING_ANIM_SPEED, RUNNING_ANIM_SPEED, moveType);
			_animator.SetFloat(_moveCycleSpeedParam, speed * animSpeed);
			_animator.SetFloat(_moveTypeParam, moveType);
		}

	}

	float UpdateAndGetIdleWeight(bool moving)
	{
		// Update these right away so we have some value immediately in a state change
		_timeMoving += Time.deltaTime;
		_timeIdle += Time.deltaTime;
		var timeToUse = moving ? _timeMoving : _timeIdle;

		if (moving)
		{
			_timeIdle = 0;
			return Mathf.Clamp01(1 - timeToUse / IDLE_TO_MOVE_BLEND_TIME);
		}
		else
		{
			_timeMoving = 0;
			return Mathf.Clamp01(timeToUse / IDLE_TO_MOVE_BLEND_TIME);
		}
	}

	class YingLayer
	{
		public YingLayer(string name, Animator animator)
		{
			LayerIndex = animator.GetLayerIndex(name);
			OriginalWeight = animator.GetLayerWeight(LayerIndex);
		}
		public int LayerIndex { get; }
		public float OriginalWeight { get; }
	}
}
