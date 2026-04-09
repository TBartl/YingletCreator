using Character.Creator;
using Character.Data;
using UnityEngine;



public class ApplySliderAsPosition : MonoBehaviour, IApplyableCustomization
{
	[SerializeField] AssetReferenceT<CharacterSliderId> _sliderReference;
	[SerializeField] Transform _target;
	[SerializeField] Vector3 _minOffset = Vector3.zero;
	[SerializeField] Vector3 _maxOffset = Vector3.zero;

	ICustomizationDataRepository _dataRepository;

	private void Awake()
	{
		_dataRepository = GetComponentInParent<ICustomizationDataRepository>();
	}

	public void Apply()
	{
		var value = GetOffset();
		_target.transform.position += _target.transform.TransformVector(value);

	}

	Vector3 GetOffset()
	{
		var sliderValue = _dataRepository.GetSliderValue(_sliderReference.LoadSync());
		var middle = 0.5f;
		if (sliderValue < middle)
		{
			float p = sliderValue / middle;
			return Vector3.LerpUnclamped(_minOffset, Vector3.zero, p);
		}
		else
		{
			float p = (sliderValue - middle) / (1 - middle);
			return Vector3.LerpUnclamped(Vector3.zero, _maxOffset, p);
		}
	}

}
