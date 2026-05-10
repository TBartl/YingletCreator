using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Bridges the gap between the Animator and any logic that wants to drive it
/// Designed to handle things like getting layer / clip IDs
/// </summary>
public interface IYingletAnimationBridge
{
	public void SetAnimState(YingletAnimState state);

	/// <summary>
	/// How fast the move cycle animation should be playing
	/// </summary>
	public void SetMoveCycleSpeed(float horizontalSpeed);

	/// <summary>
	/// Which move cycle animation to play (0 = walk, 1 = run)
	/// </summary>
	public void SetMoveType(float moveType);

	/// <summary>
	/// If the user is rising (1) or falling (0). Only relevant in the air
	/// </summary>
	public void SetRising(float rising);

	/// <summary>
	/// How activated the fall impact layer should be
	/// </summary>
	public void SetFallImpactWeight(float weight);

	/// <summary>
	/// Returns a value of how far along the move cycle animation is
	/// This is a number where each whole number represents a full loop of the animation
	/// Returns null if we're not playing the move cycle
	/// </summary>
	public float? GetMovingAnimTime();

	public void SetEncounterPose(AnimationClip clip);
}

public enum YingletAnimState
{
	Idle,
	Moving,
	Airborne,
	Sleeping
}

public class YingletAnimationBridge : MonoBehaviour, IYingletAnimationBridge
{
	[SerializeField] float STATE_CHANGE_BLEND_TIME = 0.3f;

	static readonly string[] IDLE_LAYER_NAMES = new string[] { "TailWagging", "LookAround", "EarWiggle" };
	static readonly string FALL_IMPACT_LAYER_NAME = "FallImpact";
	static readonly string ENCOUNTER_POSE_LAYER_NAME = "EncounterPose";

	static readonly int MOVE_CYCLE_SPEED_PARAM = Animator.StringToHash("MoveCycleSpeed");
	static readonly int MOVE_TYPE_PARAM = Animator.StringToHash("MoveType");
	static readonly int RISING_PARAM = Animator.StringToHash("Rising");

	static readonly int STATE_IDLE_ANIM = Animator.StringToHash("Idle");
	static readonly int STATE_MOVING_ANIM = Animator.StringToHash("Moving");
	static readonly int STATE_AIRBORNE_ANIM = Animator.StringToHash("Airborne");
	static readonly int STATE_SLEEPING_ANIM = Animator.StringToHash("Sleeping");

	private Animator _animator;

	// The idle state is a bit special in that it has a few layers on top of it that we need to disable in addition to moving off the animation
	// Keep track of those layers so we can transition them in and out
	private YingLayer[] _idleLayers;
	private YingLayer _fallImpactLayer;
	private YingLayer _encounterPoseLayer;

	YingletAnimState _currentState = YingletAnimState.Idle;
	private Coroutine _idleBlendCoroutine;
	private AnimatorOverrideController _overrideController;
	private AnimationClip _originalEncounterClip;

	private void Awake()
	{
		_animator = this.GetComponent<Animator>();
		_idleLayers = IDLE_LAYER_NAMES.Select(layerName => new YingLayer(layerName, _animator)).ToArray();
		_fallImpactLayer = new YingLayer(FALL_IMPACT_LAYER_NAME, _animator);
		_encounterPoseLayer = new YingLayer(ENCOUNTER_POSE_LAYER_NAME, _animator);

		_animator.SetLayerWeight(_fallImpactLayer.LayerIndex, 0); // Default to 0


		var originalController = _animator.runtimeAnimatorController;
		_overrideController = new AnimatorOverrideController(originalController);
		_animator.runtimeAnimatorController = _overrideController;
		_originalEncounterClip = _animator.GetCurrentAnimatorClipInfo(_encounterPoseLayer.LayerIndex).First().clip;
	}

	public void SetAnimState(YingletAnimState state)
	{
		if (_currentState == state) return;
		var lastState = _currentState;
		_currentState = state;

		_animator.CrossFadeInFixedTime(GetAnimForState(state), STATE_CHANGE_BLEND_TIME);

		// Idle state has some extra layers that need to be blended in and out, so handle that with a coroutine
		if (state == YingletAnimState.Idle)
		{
			this.StopAndStartCoroutine(ref _idleBlendCoroutine, CrossFadeIdleLayers(true));
		}
		else if (lastState == YingletAnimState.Idle)
		{
			this.StopAndStartCoroutine(ref _idleBlendCoroutine, CrossFadeIdleLayers(false));
		}
	}

	private int GetAnimForState(YingletAnimState state)
	{
		return state switch
		{
			YingletAnimState.Idle => STATE_IDLE_ANIM,
			YingletAnimState.Moving => STATE_MOVING_ANIM,
			YingletAnimState.Airborne => STATE_AIRBORNE_ANIM,
			YingletAnimState.Sleeping => STATE_SLEEPING_ANIM,
			_ => throw new System.Exception($"Unsupported state {state}")
		};
	}

	private void OnDisable()
	{
		// Incase disabling stopped the coroutines
		SetIdleLayerWeights(_currentState == YingletAnimState.Idle ? 1 : 0);
	}

	public void SetMoveCycleSpeed(float horizontalSpeed)
	{
		_animator.SetFloat(MOVE_CYCLE_SPEED_PARAM, horizontalSpeed);
	}

	public void SetMoveType(float moveType)
	{
		_animator.SetFloat(MOVE_TYPE_PARAM, moveType);
	}

	public void SetRising(float rising)
	{
		_animator.SetFloat(RISING_PARAM, rising);
	}

	IEnumerator CrossFadeIdleLayers(bool toIdle)
	{
		for (float t = Time.deltaTime; t < STATE_CHANGE_BLEND_TIME; t += Time.deltaTime)
		{
			float p = t / STATE_CHANGE_BLEND_TIME;
			if (!toIdle) p = 1 - p;
			SetIdleLayerWeights(p);
			yield return null;
		}
		SetIdleLayerWeights(toIdle ? 1 : 0);
	}

	void SetIdleLayerWeights(float weight)
	{
		foreach (var layer in _idleLayers)
		{
			_animator.SetLayerWeight(layer.LayerIndex, Mathf.Lerp(0, layer.OriginalWeight, weight));
		}
	}

	public void SetFallImpactWeight(float weight)
	{
		_animator.SetLayerWeight(_fallImpactLayer.LayerIndex, Mathf.Lerp(0, _fallImpactLayer.OriginalWeight, weight));
	}

	public float? GetMovingAnimTime()
	{
		var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

		if (stateInfo.shortNameHash != STATE_MOVING_ANIM)
		{
			return null;
		}
		return stateInfo.normalizedTime;
	}

	public void SetEncounterPose(AnimationClip clip)
	{
		_animator.SetLayerWeight(_encounterPoseLayer.LayerIndex, clip != null ? 1 : 0);
		_overrideController.ApplyOverrides(new List<KeyValuePair<AnimationClip, AnimationClip>>() { new(_originalEncounterClip, clip) });
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
