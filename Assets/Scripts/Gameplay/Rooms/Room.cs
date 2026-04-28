using UnityEngine;

public interface IRoom
{
	Vector2Int Position { get; }
	RoomOpeningsDefinition Openings { get; }
}


[System.Serializable]
public struct RoomOpeningsDefinition
{
	public bool North;
	public bool East;
	public bool South;
	public bool West;
}

public class Room : MonoBehaviour, IRoom, IInitializable
{
	[SerializeField] private RoomOpeningsDefinition _openings;

	public Vector2Int Position { get; private set; }

	public RoomOpeningsDefinition Openings => _openings;

	public void Initialize()
	{
		var pos = this.transform.localPosition;
		Position = new Vector2Int(Mathf.RoundToInt(pos.x / RoomManager.ROOM_SIZE), Mathf.RoundToInt(pos.z / RoomManager.ROOM_SIZE));
	}
}
