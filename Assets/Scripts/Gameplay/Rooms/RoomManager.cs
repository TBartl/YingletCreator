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
	[SerializeField] GameObject _passagePrefab;

	public const int ROOM_SIZE = 6;

	Dictionary<Vector2Int, IRoom> _rooms = new Dictionary<Vector2Int, IRoom>();

	public Vector2IntRange Range { get; private set; }

	public void Initialize()
	{
		// This should eventually have some sort of generator or something
		FindRooms();
		GeneratePassages();
	}

	void FindRooms()
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

	private void GeneratePassages()
	{
		foreach (var room in _rooms.Values)
		{
			CheckAndCreatePassage(room, CardinalDirection.North, new Vector2Int(0, 1));
			CheckAndCreatePassage(room, CardinalDirection.East, new Vector2Int(1, 0));
		}
	}

	private void CheckAndCreatePassage(IRoom room, CardinalDirection direction, Vector2Int adjacentOffset)
	{
		// Check if we have an opening
		if (!PassageUtils.HasOpening(room, direction)) return;

		// Check if there's a room in that spot
		Vector2Int adjacentPosition = room.Position + adjacentOffset;
		if (!_rooms.TryGetValue(adjacentPosition, out var adjacentRoom))
		{
			Debug.LogWarning($"Room at position {room.Position} has an opening to the {direction}, but no adjacent room found at {adjacentPosition}.");
			return;
		}


		// Check if the adjacent room has an opening in the opposite direction
		CardinalDirection oppositeDirection = PassageUtils.GetOppositeDirection(direction);
		if (!PassageUtils.HasOpening(adjacentRoom, oppositeDirection))
		{
			Debug.LogWarning($"Room at position {adjacentPosition} has an opening to the {oppositeDirection}, but no opening found in the adjacent room.");
			return;
		}

		// Create the passage
		var go = Instantiate(_passagePrefab, PassageUtils.CalculatePassagePosition(room, adjacentRoom), Quaternion.identity, this.transform);
		var passage = go.GetComponentSafe<IPassage>();
		passage.SetRooms(room, adjacentRoom);
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
