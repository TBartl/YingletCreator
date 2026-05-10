

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_RoundTransition : ReactiveBehaviour, ICameraControlProvider, IInitializable
{
	[SerializeField] Vector3 _posOffset;
	[SerializeField] Vector3 _rotation;
	private CameraControlProvider_FollowPlayer _followPlayer;
	private IGlobalRoundProvider _globalRoundProvider;
	private Computed<bool> _transitioning;

	public bool WantsControl => _transitioning.Val;

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (_followPlayer.CalculateTransform().Item1 + _posOffset, Quaternion.Euler(_rotation));
	}

	public void Initialize()
	{
		_followPlayer = this.GetComponentSafe<CameraControlProvider_FollowPlayer>();

		_globalRoundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		_transitioning = CreateComputed(ComputeTransitioning);
	}

	bool ComputeTransitioning()
	{
		var roundManager = _globalRoundProvider.RoundManager;
		if (roundManager == null) return false;

		var transitionState = roundManager.TransitionState.Val;
		return transitionState == RoundTransitionState.TransitioningIn || transitionState == RoundTransitionState.IncrementRound;
	}
}
