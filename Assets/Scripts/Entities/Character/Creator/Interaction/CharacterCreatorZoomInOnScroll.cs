using Character.Creator.UI;
using UnityEngine;

// Note: I think this script is dead

public class CharacterCreatorZoomInOnScroll : MonoBehaviour
{
	[SerializeField] Vector3 _zoomPos;
	[SerializeField] Vector3 _zoomRot;
	[SerializeField] float _scrollSensitivity = 1f;
	[SerializeField] float _posSpringTime = 0.3f;
	[SerializeField] float _rotSpringTime = 0.3f;
	[SerializeField] Vector3 _frameOffset;

	ISettingsManager _settingsManager;
	IUiHoverManager _uiHoverManager;
	IInPoseModeChecker _inPoseMode;
	IYingletHeightProvider _heightProvider;
	Vector3 _startPos;
	Quaternion _startRot;
	Quaternion _zoomRotQuaternion;
	float _percent = 0f;
	private Vector3 _currentPosVel;
	private Vector4 _currentRotVel;

	void Start()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		_uiHoverManager = Singletons.GetSingleton<IUiHoverManager>();
		_inPoseMode = this.GetCharacterCreatorComponent<IInPoseModeChecker>();
		_heightProvider = this.GetCharacterCreatorComponent<IYingletHeightProvider>();
		_startPos = transform.localPosition;
		_startRot = transform.localRotation;
		_zoomRotQuaternion = Quaternion.Euler(_zoomRot);
	}

	void LateUpdate()
	{
		// Early return if we're in photo mode
		if (_inPoseMode.InPoseMode.Val) return;

		UpdateTargetPercent();
		UpdatePos();
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
	private void UpdatePos()
	{
		var zoomOffset = new Vector3(0, _heightProvider.YScale - 1, 0);
		Vector3 targetPosition = Vector3.Lerp(GetFromPosition(), _zoomPos + zoomOffset, _percent);
		Quaternion targetRotation = Quaternion.Lerp(_startRot, _zoomRotQuaternion, _percent);
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentPosVel, _posSpringTime);
		transform.rotation = transform.rotation.SmoothDamp(targetRotation, ref _currentRotVel, _rotSpringTime);
	}

	Vector3 GetFromPosition()
	{
		if (_settingsManager.Settings.DefaultCameraPosition == DefaultCameraPosition.Static)
		{
			return _startPos;
		}
		else
		{
			return _startPos + _frameOffset * Mathf.Max(_heightProvider.YScale - 1, -.99f);
		}
	}

}
