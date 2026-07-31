using UnityEngine;

public interface IRoom
{
	Vector2Int Position { get; }
	Vector3 WorldPosition { get; }
	CardinalDirection Rotation { get; }
	RoomOpeningsDefinition Openings { get; }

	AudioClip Music { get; }
	T GetComponentSafe<T>();
}

public class Room : MonoBehaviour, IRoom, IInitializable
{
	[SerializeField] private RoomOpeningsDefinition _openings;
	[SerializeField] AudioClip _music;

	public Vector2Int Position { get; private set; }
	public Vector3 WorldPosition => this.transform.position;
	public CardinalDirection Rotation { get; set; }

	public RoomOpeningsDefinition Openings => _openings;
	public AudioClip Music => _music;

	public void Initialize()
	{
		Position = GetRoomPosFromWorldPos(this.transform.localPosition);

		// Determine rotation based on the transform's rotation
		var angle = (this.transform.localEulerAngles.y + 360 * 2) % 360;
		Rotation = PassageUtils.GetCardinalDirectionFromAngle(angle);
	}

	public static Vector2Int GetRoomPosFromWorldPos(Vector3 worldPos)
	{
		return new Vector2Int(Mathf.RoundToInt(worldPos.x / RoomManager.ROOM_SIZE), Mathf.RoundToInt(worldPos.z / RoomManager.ROOM_SIZE));
	}

	public T GetComponentSafe<T>()
	{
		// Optimization opportunity: Could cache this
		return gameObject.GetComponentSafe<T>();
	}
}
