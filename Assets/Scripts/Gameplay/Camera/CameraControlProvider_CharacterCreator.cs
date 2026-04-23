using Reactivity;
using UnityEngine;

internal class CameraControlProvider_CharacterCreator : ReactiveBehaviour, ICameraControlProvider
{
	[SerializeField] Vector3 _startOffset;
	[SerializeField] Vector3 _startRotation;
	[SerializeField] Vector3 _zoomOffset;
	[SerializeField] Vector3 _zoomRot;
	[SerializeField] float _scrollSensitivity = 1f;
	[SerializeField] float _posSpringTime = 0.3f;
	[SerializeField] float _rotSpringTime = 0.3f;
	[SerializeField] Vector3 _frameOffset;

	ISettingsManager _settingsManager;
	private ICharacterCreatorTracker _characterCreatorTracker;
	IUiHoverManager _uiHoverManager;
	IYingletHeightProvider _heightProvider;
	private IActiveCharacterProvider _activeCharacterProvider;
	private Quaternion _startRotQuaternion;
	private Quaternion _zoomRotQuaternion;
	float _percent = 0f;
	private Vector3 _pos;
	private Quaternion _rot;
	private Vector3 _currentPosVel;
	private Vector4 _currentRotVel;

	public bool WantsControl => _characterCreatorTracker.IsInCharacterCreator.Val;

	private void Awake()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_uiHoverManager = Singletons.GetSingleton<IUiHoverManager>();
		_heightProvider = Singletons.GetSingleton<IYingletHeightProvider>();
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();

		_characterCreatorTracker.IsInCharacterCreator.OnChanged += InCharacterCreator_OnChanged;
	}


	void Start()
	{
		_startRotQuaternion = Quaternion.Euler(_startRotation);
		_zoomRotQuaternion = Quaternion.Euler(_zoomRot);

	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		_characterCreatorTracker.IsInCharacterCreator.OnChanged -= InCharacterCreator_OnChanged;
	}

	private void InCharacterCreator_OnChanged(bool from, bool to)
	{
		if (!to) return;
		var (idealPos, idealRot) = CalculateIdealTransform();
		_pos = idealPos;
		_rot = idealRot;
	}

	void UpdateTargetPercent()
	{
		// Early return if we're hovering over UI
		if (_uiHoverManager.HoveringUi) return;

		float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
		if (Mathf.Abs(scroll) > 0.0001f)
		{
			_percent += scroll * _scrollSensitivity;
			_percent = Mathf.Clamp01(_percent);
		}
	}

	(Vector3, Quaternion) CalculateIdealTransform()
	{
		var myCharacter = _activeCharacterProvider.ActiveCharacter.Val;
		if (myCharacter == null)
		{
			Debug.LogWarning($"CameraControlProvider_CharacterCreator: MyCharacter is null");
			return (Vector3.zero, Quaternion.identity);

		}
		Vector3 characterPosition = myCharacter.transform.position;
		Vector3 offset = Vector3.Lerp(GetMinZoomOffset(), GetMaxZoomOffset(), _percent);

		Vector3 targetPosition = characterPosition + offset;
		Quaternion targetRotation = Quaternion.Lerp(_startRotQuaternion, _zoomRotQuaternion, _percent);

		return (targetPosition, targetRotation);
	}

	public (Vector3, Quaternion) CalculateTransform()
	{
		UpdateTargetPercent();

		(Vector3 targetPosition, Quaternion targetRotation) = CalculateIdealTransform();

		_pos = Vector3.SmoothDamp(_pos, targetPosition, ref _currentPosVel, _posSpringTime);
		_rot = _rot.SmoothDamp(targetRotation, ref _currentRotVel, _rotSpringTime);

		return (_pos, _rot);
	}

	Vector3 GetMinZoomOffset()
	{
		if (_settingsManager.Settings.DefaultCameraPosition == DefaultCameraPosition.Static)
		{
			return _startOffset;
		}
		else
		{
			return _startOffset + _frameOffset * Mathf.Max(_heightProvider.YScale - 1, -.99f);
		}
	}

	Vector3 GetMaxZoomOffset()
	{
		var zoomOffset = new Vector3(0, _heightProvider.YScale - 1, 0);
		return _zoomOffset + zoomOffset;
	}
}
