using Reactivity;

/// <summary>
/// Enables or disables the passage's collision with the active character
/// </summary>
public class PassageCollisionEnabler : ReactiveBehaviour
{
	private IActiveCharacterProvider _activeCharacterProvider;
	private ICurrentRoomProvider _activeRoomProvider;
	private IPassage _passage;
	private Computed<IRoom> _attachedRoom;

	private void Start()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_activeRoomProvider = Singletons.GetSingleton<ICurrentRoomProvider>();
		_passage = Singletons.GetSingleton<IPassage>();
		_attachedRoom = CreateComputed<IRoom>(ComputeAttachedRoom);
	}

	private IRoom ComputeAttachedRoom()
	{
		var currentRoom = _activeRoomProvider.CurrentRoom.Val;
		if (currentRoom == null) return null;

		if (currentRoom == _passage.RoomA) return _passage.RoomA;
		if (currentRoom == _passage.RoomB) return _passage.RoomB;
		return null;
	}

	//Computed<int> ComputeCost()
	//{
	//	var _activeCharacter = _activeCharacterProvider.ActiveCharacter.Val;
	//}
}
