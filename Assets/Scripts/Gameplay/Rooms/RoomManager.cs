using System.Collections.Generic;
using UnityEngine;

public interface IRoomManager
{
	Vector2IntRange Range { get; }
	IRoom GetRoom(Vector2Int position);
}


public struct Vector2IntRange
{
	public Vector2Int Min { get; }
	public Vector2Int Max { get; }
	public Vector2IntRange(Vector2Int min, Vector2Int max)
	{
		Min = min;
		Max = max;
	}
}

public class RoomManager : MonoBehaviour, IRoomManager, IInitializable
{
	public const int ROOM_SIZE = 6;

	Dictionary<Vector2Int, IRoom> _rooms = new Dictionary<Vector2Int, IRoom>();

	public Vector2IntRange Range { get; private set; }

	public void Initialize()
	{
		var rooms = this.GetComponentsInChildrenSafe<IRoom>();
		Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
		Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);

		if (rooms.Length == 0)
		{
			Debug.LogWarning("No rooms found in children of RoomManager.");
			Range = new Vector2IntRange(Vector2Int.zero, Vector2Int.zero);
			return;
		}

		foreach (var room in rooms)
		{
			if (_rooms.ContainsKey(room.Position))
			{
				Debug.LogError($"Duplicate room position detected: {room.Position}");
				continue;
			}
			var position = room.Position;
			_rooms.Add(room.Position, room);
			min = Vector2Int.Min(min, position);
			max = Vector2Int.Max(max, position);
		}
		Range = new Vector2IntRange(min, max);
	}

	public IRoom GetRoom(Vector2Int position)
	{
		if (_rooms.TryGetValue(position, out var room))
		{
			return room;
		}
		Debug.LogWarning($"Room at position {position} not found.");
		return null;
	}
}
