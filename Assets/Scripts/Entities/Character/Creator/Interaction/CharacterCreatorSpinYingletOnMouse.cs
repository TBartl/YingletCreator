using Networking;
using System;
using UnityEngine;

public class CharacterCreatorSpinYingletOnMouse : MonoBehaviour
{
	[SerializeField] float _spinSensitivity = 10f;
	[SerializeField] float _startRotation = -140f;
	private ICharacterCreatorTracker _characterCreatorTracker;
	private IRotateToVelocity _rotateToVelocity;
	private ICharacterIdentity _identity;

	IDisposable _suspendAutoRotation;

	private void Awake()
	{
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_rotateToVelocity = this.GetComponentSafe<IRotateToVelocity>();

		_identity = this.GetComponentInParentSafe<ICharacterIdentity>();
		_characterCreatorTracker.IsInCharacterCreator.OnChanged += IsInCharacterCreator_OnChanged;
	}

	private void OnDestroy()
	{
		_characterCreatorTracker.IsInCharacterCreator.OnChanged -= IsInCharacterCreator_OnChanged;
	}

	private void IsInCharacterCreator_OnChanged(bool arg1, bool to)
	{
		if (to && _identity.IsActiveAndMine)
		{
			this.transform.rotation = Quaternion.Euler(0, _startRotation, 0);

			if (_suspendAutoRotation == null) _suspendAutoRotation = _rotateToVelocity.SuspendAutoRotation();
		}
		else
		{
			_suspendAutoRotation?.Dispose();
			_suspendAutoRotation = null;
		}
	}

	void Update()
	{
		if (!_characterCreatorTracker.IsInCharacterCreator.Val) return;

		if (Input.GetMouseButton(1))
		{
			float spinAmount = Input.GetAxisRaw("Mouse X") * _spinSensitivity;
			this.transform.rotation *= Quaternion.Euler(0, spinAmount, 0);
		}

	}
}
