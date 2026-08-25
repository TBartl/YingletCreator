using Character.Data;
using Reactivity;
using UnityEngine;


public class ApplyHatOffset : ReactiveBehaviour, IApplyableCustomization
{
	[SerializeField] Transform _target;

	private ICharacterToggleProvider _toggleProvider;
	private Computed<float> _computeOffset;

	private void Awake()
	{
		_toggleProvider = this.GetComponentInParentSafe<ICharacterToggleProvider>();
		_computeOffset = CreateComputed(ComputeOffset);
	}

	private float ComputeOffset()
	{
		var toggles = _toggleProvider.Toggles;
		foreach (var toggle in toggles)
		{
			foreach (var component in toggle.Components)
			{
				if (component is IOffsetHatBone offsetHatBone)
				{
					return offsetHatBone.Amount;
				}
			}
		}
		return 0;
	}

	public void Apply()
	{
		var offset = _computeOffset.Val;
		_target.transform.localPosition += Vector3.up * offset;

	}

}
