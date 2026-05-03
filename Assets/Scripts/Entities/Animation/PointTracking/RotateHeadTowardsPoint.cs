using UnityEngine;

public class RotateHeadTowardsPoint : MonoBehaviour
{
	[SerializeField] Transform _rootHeadBone;
	[SerializeField] Transform _headCenter;

	[SerializeField] AnimationCurve _realAngleToLimitedAngle;

	[Tooltip("This corresponds to how much weight each bone down from the root should rotate by. This should not exceed 1 in either axis. No z-axis because we don't want to spin the head like that")]
	[SerializeField] Vector2[] _chainWeights;

	[Header("Tuning")]
	[SerializeField] float SpringStrength = 40f;
	[SerializeField] float Damping = 8f;
	private Vector3 _velocity;
	private Vector3 _currentEulerAngles;


	private IPointTrackingLocationProvider _locationProvider;
	private IPointTrackingWeightProvider _weightProvider;
	private Transform[] _boneChain;

	private void Awake()
	{
		_locationProvider = this.GetComponentInParentSafe<IPointTrackingLocationProvider>();
		_weightProvider = this.GetComponentInParentSafe<IPointTrackingWeightProvider>();

		// Collect bone chain upward from head
		_boneChain = new Transform[_chainWeights.Length];
		var current = _rootHeadBone;
		for (int i = 0; i < _chainWeights.Length; i++)
		{
			_boneChain[i] = current;
			current = current.parent;
		}
	}

	private void LateUpdate()
	{
		if (_weightProvider.Weight < 0.001f || _locationProvider.MoveUpperBody == false)
		{
			_currentEulerAngles = Vector3.forward;
			return;
		}
		RotateChainTowards(_locationProvider.Position);
	}

	private void RotateChainTowards(Vector3 targetPosition)
	{
		// Figure out how much we ideally want to rotate by
		var baseRotation = _rootHeadBone.rotation;
		var targetRotation = Quaternion.LookRotation(targetPosition - _headCenter.position, Vector3.up);
		var rotationAmount = Quaternion.Inverse(baseRotation) * targetRotation;

		// We don't want characters turning super far, so clamp down on the possible angle
		rotationAmount.ToAngleAxis(out float angle, out Vector3 axis);
		if (angle > 180f) angle -= 360f; // normalize angle range
		float limitedAngle = Mathf.Sign(angle) * _realAngleToLimitedAngle.Evaluate(Mathf.Abs(angle));
		var clampedRotationAmount = Quaternion.AngleAxis(limitedAngle, axis);

		var eulerRotationAmount = clampedRotationAmount.eulerAngles.DirectionalizeEulerAngles();
		MathUtils.SpringDampTowards(ref _currentEulerAngles, ref _velocity, eulerRotationAmount, SpringStrength, Damping);

		for (int i = 0; i < _boneChain.Length; i++)
		{
			var bone = _boneChain[i];
			var weight = _chainWeights[i] * _weightProvider.Weight;
			var weightedRot = Quaternion.Euler(_currentEulerAngles.x * weight.x, _currentEulerAngles.y * weight.y, 0);
			bone.localRotation = bone.localRotation * weightedRot;
		}
	}
}