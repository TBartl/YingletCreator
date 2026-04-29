using Reactivity;
using UnityEngine;

public class HidePassageTopWhenObstructing : ReactiveBehaviour
{
	private ICurrentRoomProvider _currentRoomProvider;
	Shader CUTOFF_FROM_TOP_SHADER;
	static readonly int Y_CUTOFF_PROPERTY_ID = Shader.PropertyToID("_YCutoff");

	private Computed<bool> _show;
	private IRoom _roomToObstructIn;

	private void Start()
	{
		var room = this.GetComponentInParentSafe<IRoom>();
		var localPositionRelativeToRoom = this.transform.position - room.WorldPosition;
		if (Mathf.Abs(localPositionRelativeToRoom.x) > 1)
		{
			// We're not a vertical passage. This will never obstruct noticeably, so just remove this component
			Destroy(this);
			return;
		}

		CUTOFF_FROM_TOP_SHADER = Shader.Find("Shader Graphs/CutOffFromTop");
		var roomManager = this.GetExpeditionComponent<IRoomManager>();
		_currentRoomProvider = Singletons.GetSingleton<ICurrentRoomProvider>();
		_show = CreateComputed(ComputeShow);

		var expectedRoomPos = Room.GetRoomPosFromWorldPos(this.transform.position + new Vector3(0, 0, RoomManager.ROOM_SIZE / 2));
		_roomToObstructIn = roomManager.GetRoom(expectedRoomPos);

		AddReflector(Reflect);
	}

	private bool ComputeShow()
	{
		return _currentRoomProvider.CurrentRoom.Val != _roomToObstructIn;
	}

	private void Reflect()
	{
		this.gameObject.SetActive(_show.Val);
	}
}
