using Character.Data;
using Reactivity;

internal class CharacterStanceAnimation : ReactiveBehaviour
{
	private IYingletAnimationBridge _animBridge;
	private ICharacterClipProvider _clipProvider;
	Computed<PoseId> _currentEncounterPose;

	private void Start()
	{
		_animBridge = this.GetComponentSafe<IYingletAnimationBridge>();
		_clipProvider = this.GetComponentInParentSafe<ICharacterClipProvider>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var idleAnim = _clipProvider.Stance.IdleAnim;
		_animBridge.SetIdleAnim(idleAnim);
	}
}
