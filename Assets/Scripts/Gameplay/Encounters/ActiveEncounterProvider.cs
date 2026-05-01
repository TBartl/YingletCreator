using Reactivity;

public interface IActiveEncounterProvider
{
	IReadOnlyObservable<IEncounterInstance> ActiveEncounter { get; }
}

public class ActiveEncounterProvider : ReactiveBehaviour, IActiveEncounterProvider, IInitializable
{
	private IActiveCharacterProvider _activeCharacterProvider;
	private Computed<ICharacterEncounterReference> _activeEncounterReference;
	Computed<IEncounterInstance> _activeEncounter;
	public IReadOnlyObservable<IEncounterInstance> ActiveEncounter => _activeEncounter;

	public void Initialize()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_activeEncounterReference = CreateComputed(() =>
		{
			var activeCharacter = _activeCharacterProvider.ActiveExpeditionCharacter.Val;
			if (activeCharacter == null) return null;
			return activeCharacter.GetComponentInChildrenSafe<ICharacterEncounterReference>();
		});
		_activeEncounter = CreateComputed(() =>
		{
			return _activeEncounterReference.Val?.Encounter?.Val;
		});
	}
}
