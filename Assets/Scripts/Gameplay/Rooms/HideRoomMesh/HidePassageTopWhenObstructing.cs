using Reactivity;
using UnityEngine;

[RequireComponent(typeof(RoomMeshVisibilityTransitioner))]
public class HidePassageTopWhenObstructing : ReactiveBehaviour
{
	private IRoomMeshVisibilityConditions _associatedRoom;
	private IRoomMeshVisibilityTransitioner _visibilityTransitioner;

	private void Start()
	{
		// See if we're a horizontal passage. If so, this will never be applicable
		var room = this.GetComponentInParentSafe<IRoom>();
		var localPositionRelativeToRoom = this.transform.position - room.WorldPosition;
		if (Mathf.Abs(localPositionRelativeToRoom.x) > 1)
		{
			Destroy(this);
			return;
		}

		var roomManager = this.GetExpeditionComponent<IRoomManager>();

		// Calculate the room
		var expectedRoomPos = Room.GetRoomPosFromWorldPos(this.transform.position + new Vector3(0, 0, RoomManager.ROOM_SIZE / 2));
		_associatedRoom = roomManager.GetRoom(expectedRoomPos)?.GetComponentSafe<IRoomMeshVisibilityConditions>();
		if (_associatedRoom == null)
		{
			Debug.LogWarning($"Could not find associated room for {this.name} at {expectedRoomPos}.");
			Destroy(this);
			return;
		}

		_visibilityTransitioner = this.GetComponentSafe<IRoomMeshVisibilityTransitioner>();

		var show = _associatedRoom.ShowBottomPassage;
		show.OnChanged += Show_OnChanged;

		_visibilityTransitioner.ForceTo(show.Val);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_associatedRoom != null)
		{
			_associatedRoom.ShowBottomPassage.OnChanged -= Show_OnChanged;
		}
	}

	private void Show_OnChanged(bool from, bool to)
	{
		_visibilityTransitioner.TransitionTo(to);
	}
}
