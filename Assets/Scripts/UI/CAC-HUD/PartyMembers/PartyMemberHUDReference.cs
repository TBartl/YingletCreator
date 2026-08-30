using Reactivity;

public interface IPartyMemberHUDReference
{
	ICharacterRoot Character { get; }
	IReadOnlyObservable<ICharacterRoot> CharacterObservable { get; }
}

public interface IWriteablePartyMemberHUDReference : IPartyMemberHUDReference
{
	void SetCharacter(ICharacterRoot character);
}

public class PartyMemberHUDReference : ReactiveBehaviour, IWriteablePartyMemberHUDReference, IClassReference, IInitializable, ISelectable
{
	private IActiveCharacterProvider _activeCharacterProvider;
	Observable<ICharacterRoot> _character = new Observable<ICharacterRoot>();
	Computed<bool> _selected;
	Computed<ClassId> _class;

	public ICharacterRoot Character => _character.Val;
	public IReadOnlyObservable<ICharacterRoot> CharacterObservable => _character;

	public ClassId Class => _class.Val;

	public void SetCharacter(ICharacterRoot character)
	{
		_character.Val = character;
	}

	public IReadOnlyObservable<bool> Selected => _selected;


	public void Initialize()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_selected = CreateComputed(ComputeSelected);
		_class = CreateComputed(ComputeClass);
	}

	private bool ComputeSelected()
	{
		return _activeCharacterProvider.ActiveCharacter.Val == _character.Val;
	}

	private ClassId ComputeClass()
	{
		var character = Character;
		if (character == null) return null;
		var classRepo = character.GetComponentInChildrenSafe<IClassReference>();
		return classRepo.Class;
	}
}
