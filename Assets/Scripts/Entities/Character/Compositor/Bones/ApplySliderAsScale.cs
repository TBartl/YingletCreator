using UnityEngine;

public class ApplySliderAsScale : ApplySliderAsScaleBase, IApplyableCustomization
{
	[SerializeField] Transform _target;

	public void Apply()
	{
		var size = GetSize();
		ApplyToTarget(_target, size);
	}
}
