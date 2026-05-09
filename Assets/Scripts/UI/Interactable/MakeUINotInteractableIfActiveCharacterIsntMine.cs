using Networking;
using Reactivity;

public class MakeUINotInteractableIfActiveCharacterIsntMine : ReactiveBehaviour, IUIInteractable, IInitializable
{
	private IActiveCharacterProvider _characterProvider;
	Computed<bool> _interactable;
	public IReadOnlyObservable<bool> Interactable => _interactable;

	public void Initialize()
	{
		_characterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_interactable = CreateComputed(ComputeInteractable);
	}

	private bool ComputeInteractable()
	{
		var activeCharacter = _characterProvider.ActiveCharacter.Val;
		if (activeCharacter == null) return false;
		return activeCharacter.GetComponentSafe<ICharacterIdentity>().IsMine;
	}
}
