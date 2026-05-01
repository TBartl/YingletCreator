using Reactivity;

public interface IActiveRoomProvider
{
	IReadOnlyObservable<IRoom> ActiveRoom { get; }
}

public class ActiveRoomProvider : ReactiveBehaviour, IActiveRoomProvider, IInitializable
{
	private IActiveCharacterProvider _activeCharacterProvider;
	private Computed<ICharacterRoomDetector> _activeRoomDetector;
	Computed<IRoom> _activeRoom;
	public IReadOnlyObservable<IRoom> ActiveRoom => _activeRoom;

	public void Initialize()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_activeRoomDetector = CreateComputed(() =>
		{
			var activeCharacter = _activeCharacterProvider.ActiveExpeditionCharacter.Val;
			if (activeCharacter == null) return null;
			return activeCharacter.GetComponentInChildrenSafe<ICharacterRoomDetector>();
		});
		_activeRoom = CreateComputed(() =>
		{
			return _activeRoomDetector.Val?.CurrentRoom?.Val;
		});
	}
}
