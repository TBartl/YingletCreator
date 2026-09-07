using UnityEngine;

internal sealed class PupilOffsetMutator_StanceOffset : MonoBehaviour, IPupilOffsetMutator, IInitializable
{

	// The pupil starts centered so it can be scaled. Move it to the default eye pos
	public static readonly PupilOffsets InherentOffset = new PupilOffsets(0.151f, .094f, .094f);
	private ICharacterClipProvider _clipProvider;
	private IYingletAnimationBridge _animBridge;

	// Ideally, we'd like to smooth this but w/e


	public void Initialize()
	{
		_clipProvider = this.GetComponentInParentSafe<ICharacterClipProvider>();
		_animBridge = this.GetCharacterRootComponent<IYingletAnimationBridge>();
	}

	public PupilOffsets Mutate(PupilOffsets input)
	{
		// Only apply in idle state
		if (_animBridge.AnimState != YingletAnimState.Idle) return input;

		return input.ShiftBothBy(new Vector2(0, _clipProvider.Stance.PupilOffsetY));
	}
}
