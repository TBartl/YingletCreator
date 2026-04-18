using Character.Creator;
using UnityEngine;

public class EyeExpressionMutator_PortraitOverride : MonoBehaviour, IBaseEyeExpressionMutator
{
	private ICustomizationDataRepository _dataRepo;

	void Awake()
	{
		_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();
	}


	public EyeExpression Mutate(EyeExpression input)
	{
		if (!_dataRepo.CustomizationData.PortraitData.UseOverrideExpressions.Val) return input;

		return (EyeExpression)_dataRepo.CustomizationData.PortraitData.OverrideEyeExpression.Val;
	}
}
