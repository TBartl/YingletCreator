using Character.Creator;
using UnityEngine;

internal sealed class PupilOffsetMutator_PortraitOverride : MonoBehaviour, IPupilOffsetMutator
{
	private ICustomizationDataRepository _dataRepo;

	private void Awake()
	{
		_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();
	}

	public PupilOffsets Mutate(PupilOffsets input)
	{
		var portrait = _dataRepo.CustomizationData.PortraitData.PortraitId.Val;
		if (portrait == null)
		{
			return input;
		}
		return input + portrait.PupilOffsets;
	}
}
