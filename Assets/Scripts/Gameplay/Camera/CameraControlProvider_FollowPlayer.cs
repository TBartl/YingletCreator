

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FollowPlayer : ReactiveBehaviour, ICameraControlProvider
{

	[SerializeField] float OFFSET_BACK = 5;
	[SerializeField] float OFFSET_UP = 3;
	[SerializeField] float OFFSET_PIVOT_UP = 2;
	[SerializeField] float VELOCITY_OFFSET_MULTIPLIER = 0.5f;
	[SerializeField] float LERPING_POWER = 1;
	[SerializeField] float MAX_DISTANCE = 10f;

	private IActiveCharacterProvider _activeCharacterProvider;
	Computed<Rigidbody> _localCharacterRigidbody;

	private Vector3 _pos;
	private Quaternion _rot;

	public bool WantsControl => _activeCharacterProvider.ActiveCharacter.Val != null;

	private void Start()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_localCharacterRigidbody = CreateComputed(ComputeLocalCharacterRigidbody);
	}

	private Rigidbody ComputeLocalCharacterRigidbody()
	{
		return _activeCharacterProvider.ActiveCharacter.Val?.GetComponent<Rigidbody>();
	}

	void LateUpdate()
	{
		var target = _localCharacterRigidbody.Val;
		if (target == null) return;

		var lastPos = _pos;

		var pivotPoint = target.position + Vector3.up * OFFSET_PIVOT_UP;
		pivotPoint += target.linearVelocity.WithoutY() * VELOCITY_OFFSET_MULTIPLIER; // Look ahead in the direction we're moving

		var targetPos = pivotPoint + Vector3.back * OFFSET_BACK + Vector3.up * OFFSET_UP;
		bool distanceTooFar = Vector3.Distance(_pos.WithoutY(), targetPos.WithoutY()) > MAX_DISTANCE;

		if (distanceTooFar)
		{
			_pos = targetPos;
		}
		else
		{
			_pos = Vector3.Lerp(lastPos, targetPos, LERPING_POWER * Time.deltaTime);
		}
		_rot = Quaternion.LookRotation(pivotPoint - targetPos, Vector3.up);

	}

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (_pos, _rot);
	}
}
