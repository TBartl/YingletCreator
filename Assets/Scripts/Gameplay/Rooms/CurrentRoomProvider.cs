using Reactivity;

public interface ICurrentRoomProvider
{
	IReadOnlyObservable<IRoom> CurrentRoom { get; }
}

public class CurrentRoomProvider : ReactiveBehaviour, ICurrentRoomProvider, IInitializable
{
	private IActiveCharacterProvider _activeCharacterProvider;
	Computed<IRoom> _currentRoom;
	public IReadOnlyObservable<IRoom> CurrentRoom => _currentRoom;

	public void Initialize()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_currentRoom = CreateComputed(ComputeCurrentRoom);
	}

	private IRoom ComputeCurrentRoom()
	{
		var activeCharacter = _activeCharacterProvider.ActiveCharacter.Val;
		if (activeCharacter == null) return null;
		return activeCharacter.GetComponentInChildrenSafe<ICharacterRoomDetector>().CurrentRoom.Val;
	}
}
