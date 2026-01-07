using Character.Creator;
using Character.Data;
using UnityEngine;


enum ApplySliderMode
{
	Override,
	Multiply
}

public abstract class ApplySliderAsScaleBase : MonoBehaviour
{
	const float MIDDLE = 0.5f;

	[SerializeField] AssetReferenceT<CharacterSliderId> _sliderReference;
	[SerializeField] Vector3 _minSize = Vector3.one;
	[SerializeField] Vector3 _maxSize = Vector3.one;
	[SerializeField] ApplySliderMode _applyMode;

	ICustomizationSelectedDataRepository _dataRepository;

	private void Awake()
	{
		_dataRepository = GetComponentInParent<ICustomizationSelectedDataRepository>();
	}

	public Vector3 GetSize()
	{
		var sliderValue = _dataRepository.GetSliderValue(_sliderReference.LoadSync());
		if (sliderValue < MIDDLE)
		{
			float p = sliderValue / MIDDLE;
			return Vector3.LerpUnclamped(_minSize, Vector3.one, p);
		}
		else
		{
			float p = (sliderValue - MIDDLE) / (1 - MIDDLE);
			return Vector3.LerpUnclamped(Vector3.one, _maxSize, p);
		}
	}

	public CharacterSliderId SliderId => _sliderReference.LoadSync();

	protected void ApplyToTarget(Transform target, Vector3 size)
	{
		if (_applyMode == ApplySliderMode.Override)
		{
			target.localScale = size;
		}
		else
		{
			target.localScale = target.localScale.Multiply(size);
		}
	}
}
