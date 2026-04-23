using UnityEngine;

public class CharacterCreatorSpinYingletOnMouse : MonoBehaviour
{
	[SerializeField] float _spinSensitivity = 10f;
	[SerializeField] float _startRotation = -140f;
	private ICharacterCreatorTracker _characterCreatorTracker;
	private IPlayerIdentity _identity;

	private void Awake()
	{
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_identity = this.GetComponentInParent<IPlayerIdentity>();
		_characterCreatorTracker.IsInCharacterCreator.OnChanged += IsInCharacterCreator_OnChanged;
	}

	private void OnDestroy()
	{
		_characterCreatorTracker.IsInCharacterCreator.OnChanged -= IsInCharacterCreator_OnChanged;
	}

	private void IsInCharacterCreator_OnChanged(bool arg1, bool to)
	{
		if (to && _identity.IsActive)
		{
			this.transform.rotation = Quaternion.Euler(0, _startRotation, 0);
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
