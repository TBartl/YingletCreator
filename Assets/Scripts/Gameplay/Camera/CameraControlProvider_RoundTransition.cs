

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_RoundTransition : ReactiveBehaviour, ICameraControlProvider, IInitializable
{
	[SerializeField] Vector3 _posOffset;
	[SerializeField] Vector3 _rotation;

	private Vector3 _lastPos;
	private IGlobalRoundProvider _globalRoundProvider;
	private Computed<bool> _transitioning;

	public bool WantsControl => _transitioning.Val;

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (_lastPos + _posOffset, Quaternion.Euler(_rotation));
	}

	public void Initialize()
	{
		_globalRoundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		_transitioning = CreateComputed(ComputeTransitioning);
	}

	void Update()
	{
		if (!_transitioning.Val)
		{
			_lastPos = transform.position;
		}
	}

	bool ComputeTransitioning()
	{
		var roundManager = _globalRoundProvider.RoundManager;
		if (roundManager == null) return false;

		var transitionState = roundManager.TransitionState.Val;
		return transitionState == RoundTransitionState.TransitioningIn || transitionState == RoundTransitionState.IncrementRound;
	}
}
