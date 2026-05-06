using Networking;
using Reactivity;

public class EncounterUIInteractableOnlyIfOurCharacter : ReactiveBehaviour, IUIInteractable, IInitializable
{
	private IActiveEncounterProvider _activeEncounterProvider;
	private Computed<bool> _isMine;

	public IReadOnlyObservable<bool> Interactable => _isMine;

	public void Initialize()
	{
		_activeEncounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();
		_isMine = CreateComputed<bool>(ComputeIsMine);
	}

	private bool ComputeIsMine()
	{
		var activeEncounter = _activeEncounterProvider.ActiveEncounter.Val;
		if (activeEncounter == null) return false;
		var characterIdentity = activeEncounter.Character.GetComponentSafe<ICharacterIdentity>();

		return characterIdentity.IsMine;
	}
}
