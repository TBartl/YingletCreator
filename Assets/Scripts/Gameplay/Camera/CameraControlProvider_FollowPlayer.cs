

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FollowPlayer : ReactiveBehaviour, ICameraControlProvider
{

	// The pivot point we're tracking
	[SerializeField] float OFFSET_PIVOT_UP_ZOOMED_IN = 0f;
	[SerializeField] float OFFSET_PIVOT_UP_DEFAULT = 0.4f;

	// Our offset from the pivot point
	[SerializeField] Vector3 OFFSET_CAMERA_ZOOMED_IN = new Vector3(0, .1f, -.3f);
	[SerializeField] Vector3 OFFSET_CAMERA_DEFAULT = new Vector3(0, 2.21f, -2.1f);


	[SerializeField] float VELOCITY_OFFSET_MULTIPLIER = 0.5f;
	[SerializeField] float LERPING_POWER = 1;
	[SerializeField] float MAX_DISTANCE = 10f;

	private IActiveCharacterProvider _activeCharacterProvider;
	private IYingletHeightProvider _heightProvider;
	private ICameraControl _cameraControl;
	Computed<Rigidbody> _localCharacterRigidbody;

	// Zoom
	[SerializeField] float _scrollSensitivity = 2f;
	float _zoomOutPercent = 1f;

	[SerializeField] float ROT_SPRING_TIME = 0.2f;

	private Vector3 _pos;
	private Quaternion _rot;
	private Vector4 _currentRotVel;

	public bool WantsControl => _activeCharacterProvider.ActiveCharacter.Val != null;

	private void Start()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_heightProvider = Singletons.GetSingleton<IYingletHeightProvider>();
		_cameraControl = this.GetComponentInParentSafe<ICameraControl>();
		_localCharacterRigidbody = CreateComputed(ComputeLocalCharacterRigidbody);
	}

	private Rigidbody ComputeLocalCharacterRigidbody()
	{
		return _activeCharacterProvider.ActiveCharacter.Val?.GetComponentInChildrenSafe<Rigidbody>();
	}

	void LateUpdate()
	{
		UpdateTargetPercent();

		UpdatePos();
	}

	void UpdateTargetPercent()
	{
		if (_cameraControl.CurrentProvider != (ICameraControlProvider)this) return;
		float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
		if (Mathf.Abs(scroll) > 0.0001f)
		{
			_zoomOutPercent -= scroll * _scrollSensitivity;
			_zoomOutPercent = Mathf.Clamp01(_zoomOutPercent);
		}
	}
	void UpdatePos()
	{
		var target = _localCharacterRigidbody.Val;
		if (target == null) return;

		var lastPos = _pos;

		// Start with the raw position on the XZ plane
		var pivotPoint = target.position.WithoutY();

		// Add a Y offset to move the pivot point up
		pivotPoint += GetPivotOffset();

		// Add an XZ offset based on velocity
		pivotPoint += target.linearVelocity.WithoutY() * VELOCITY_OFFSET_MULTIPLIER; // Look ahead in the direction we're moving

		// Add an offset to move the camera into place
		var targetPos = pivotPoint + GetCameraOffset();

		// If we're too far, just jump it
		bool distanceTooFar = Vector3.Distance(_pos.WithoutY(), targetPos.WithoutY()) > MAX_DISTANCE;
		if (distanceTooFar)
		{
			_pos = targetPos;
		}
		// Otherwise lerp it
		else
		{
			_pos = Vector3.Lerp(lastPos, targetPos, LERPING_POWER * Time.deltaTime);
		}

		// Look at the pivot point
		var tagetRot = Quaternion.LookRotation(pivotPoint - targetPos, Vector3.up);
		_rot = _rot.SmoothDamp(tagetRot, ref _currentRotVel, ROT_SPRING_TIME);
	}

	float GetMinPivotOffset()
	{
		return OFFSET_PIVOT_UP_ZOOMED_IN + _heightProvider.YScale;
	}
	float GetMaxPivotOffset()
	{
		return OFFSET_PIVOT_UP_DEFAULT;
	}
	Vector3 GetPivotOffset()
	{
		float y = Mathf.Lerp(GetMinPivotOffset(), GetMaxPivotOffset(), _zoomOutPercent);
		return new Vector3(0, y, 0);
	}

	Vector3 GetCameraOffset()
	{
		return Vector3.Lerp(OFFSET_CAMERA_ZOOMED_IN, OFFSET_CAMERA_DEFAULT, _zoomOutPercent);
	}

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (_pos, _rot);
	}
}
