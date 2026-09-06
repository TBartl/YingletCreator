using UnityEngine;

internal sealed class PupilOffsetMutator_PortraitOverride : MonoBehaviour, IPupilOffsetMutator
{
	private ICharacterClipProvider _portraitProvider;

	private void Awake()
	{
		_portraitProvider = this.GetComponentInParentSafe<ICharacterClipProvider>();
	}

	public PupilOffsets Mutate(PupilOffsets input)
	{
		var portrait = _portraitProvider.Portrait;
		if (portrait == null)
		{
			return input;
		}
		return input + portrait.PupilOffsets;
	}
}
