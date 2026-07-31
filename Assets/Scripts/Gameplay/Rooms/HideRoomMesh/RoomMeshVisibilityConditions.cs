using Reactivity;

/// <summary>
/// We want to hide certain parts of the room mesh depending on what's visible
/// - Only showing the ceiling when the player is in the encounter
/// - Hiding the passageway when the player is in the room
/// </summary>
public interface IRoomMeshVisibilityConditions
{
	IReadOnlyObservable<bool> ShowBottomPassage { get; }
}

public class RoomMeshVisibilityConditions : ReactiveBehaviour, IRoomMeshVisibilityConditions, IInitializable
{
	private Computed<bool> _showBottomPassage;
	private IRoom _room;
	private IActiveRoomProvider _activeRoomProvider;
	public IReadOnlyObservable<bool> ShowBottomPassage => _showBottomPassage;

	public void Initialize()
	{
		_room = this.GetComponentSafe<IRoom>();
		_activeRoomProvider = Singletons.GetSingleton<IActiveRoomProvider>();
		_showBottomPassage = CreateComputed(ComputeShowBottomPassage);
	}

	private bool ComputeShowBottomPassage()
	{
		return _activeRoomProvider.ActiveRoom.Val != _room;
	}
}
