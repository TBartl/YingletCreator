using Reactivity;

/// <summary>
/// We want to hide certain parts of the room mesh depending on what's visible
/// - Only showing the ceiling when the player is in the encounter
/// - Hiding the passageway when the player is in the room
/// </summary>
public interface IRoomMeshVisibilityConditions
{
	IReadOnlyObservable<bool> ShowBottomPassage { get; }
	IReadOnlyObservable<bool> ShowDuringEncounter { get; }
}

public class RoomMeshVisibilityConditions : ReactiveBehaviour, IRoomMeshVisibilityConditions, IInitializable
{
	private Computed<bool> _showBottomPassage;
	private Computed<bool> _showDuringEncounter;
	private IRoom _room;
	private IActiveRoomProvider _activeRoomProvider;
	private IActiveEncounterProvider _activeEncounterProvider;

	public IReadOnlyObservable<bool> ShowBottomPassage => _showBottomPassage;
	public IReadOnlyObservable<bool> ShowDuringEncounter => _showDuringEncounter;


	public void Initialize()
	{
		_room = this.GetComponentSafe<IRoom>();
		_activeRoomProvider = Singletons.GetSingleton<IActiveRoomProvider>();
		_activeEncounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();

		_showBottomPassage = CreateComputed(ComputeShowBottomPassage);
		_showDuringEncounter = CreateComputed(ComputeShowDuringEncounter);
	}

	private bool ComputeShowBottomPassage()
	{
		return _activeRoomProvider.ActiveRoom.Val != _room;
	}
	private bool ComputeShowDuringEncounter()
	{
		var encounterInstance = _activeEncounterProvider.ActiveEncounter.Val;
		if (encounterInstance == null)
		{
			return false;
		}
		return encounterInstance.Room == _room;
	}

}
