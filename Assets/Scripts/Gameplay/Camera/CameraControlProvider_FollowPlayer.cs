

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FollowPlayer : ReactiveBehaviour, ICameraControlProvider
{
	[SerializeField] Transform _target;

	[SerializeField] float OFFSET_BACK = 5;
	[SerializeField] float OFFSET_UP = 3;
	[SerializeField] float OFFSET_PIVOT_UP = 2;
	private Vector3 _targetPos;
	private Quaternion _targetRot;

	public bool WantsControl => true; // This is effectively the default for now (unless player despawns or something idk we'll figure that out)


	void LateUpdate()
	{
		if (_target == null) return;

		var pivotPoint = _target.position + Vector3.up * OFFSET_PIVOT_UP;
		_targetPos = pivotPoint - _target.forward * OFFSET_BACK + Vector3.up * OFFSET_UP;
		_targetRot = Quaternion.LookRotation(pivotPoint - _targetPos, Vector3.up);
	}

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (_targetPos, _targetRot);
	}
}
