using Reactivity;

internal class CharacterStanceAnimation : ReactiveBehaviour
{
	private IYingletAnimationBridge _animBridge;
	private ICharacterClipProvider _clipProvider;

	private void Start()
	{
		_animBridge = this.GetComponentSafe<IYingletAnimationBridge>();
		_clipProvider = this.GetComponentInParentSafe<ICharacterClipProvider>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		_animBridge.SetStanceAnims(_clipProvider.Stance);
	}
}
