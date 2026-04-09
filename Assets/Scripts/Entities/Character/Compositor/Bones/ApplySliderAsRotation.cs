using Character.Creator;
using Character.Data;
using UnityEngine;


public class ApplySliderAsRotation : MonoBehaviour, IApplyableCustomization
{
	[SerializeField] AssetReferenceT<CharacterSliderId> _sliderReference;
	[SerializeField] Transform _target;
	[SerializeField] Vector3 _eulerAngles;
	[SerializeField] AnimationCurve _applyAmountBySliderVal;

	ICustomizationDataRepository _dataRepository;

	private void Awake()
	{
		_dataRepository = GetComponentInParent<ICustomizationDataRepository>();
	}

	public void Apply()
	{
		var sliderValue = _dataRepository.GetSliderValue(_sliderReference.LoadSync());

		var p = _applyAmountBySliderVal.Evaluate(sliderValue);

		_target.transform.localRotation = _target.transform.localRotation * Quaternion.Euler(_eulerAngles * p);
	}
}
