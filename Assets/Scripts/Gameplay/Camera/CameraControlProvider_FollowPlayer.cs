

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FollowPlayer : ReactiveBehaviour, ICameraControlProvider
{

	[SerializeField] float OFFSET_BACK = 5;
	[SerializeField] float OFFSET_UP = 3;
	[SerializeField] float OFFSET_PIVOT_UP = 2;
	[SerializeField] float VELOCITY_OFFSET_MULTIPLIER = 0.5f;
	[SerializeField] float LERPING_POWER = 1;

	private ICharacterSpawner _characterSpawner;
	Computed<Rigidbody> _localCharacterRigidbody;

	private Vector3 _pos;
	private Quaternion _rot;

	public bool WantsControl => true; // This is effectively the default for now (unless player despawns or something idk we'll figure that out)

	private void Start()
	{
		_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		_localCharacterRigidbody = CreateComputed(ComputeLocalCharacterRigidbody);
	}

	private Rigidbody ComputeLocalCharacterRigidbody()
	{
		return _characterSpawner.MyCharacter?.GetComponent<Rigidbody>();
	}

	void LateUpdate()
	{
		var target = _localCharacterRigidbody.Val;
		if (target == null) return;

		var lastPos = _pos;

		var pivotPoint = target.position + Vector3.up * OFFSET_PIVOT_UP;
		pivotPoint += target.linearVelocity.WithoutY() * VELOCITY_OFFSET_MULTIPLIER; // Look ahead in the direction we're moving

		var targetPos = pivotPoint + Vector3.back * OFFSET_BACK + Vector3.up * OFFSET_UP;
		_pos = Vector3.Lerp(lastPos, targetPos, LERPING_POWER * Time.deltaTime);
		_rot = Quaternion.LookRotation(pivotPoint - targetPos, Vector3.up);
	}

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (_pos, _rot);
	}
}
