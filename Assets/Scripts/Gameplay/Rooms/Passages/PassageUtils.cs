using UnityEngine;

[System.Serializable]
public struct RoomOpeningsDefinition
{
	public bool North;
	public bool East;
	public bool South;
	public bool West;
}

public enum CardinalDirection
{
	North,
	East,
	South,
	West
}

public static class PassageUtils
{
	private const float ANGLE_TOLERANCE = 0.01f; // Allows for floating-point precision errors

	public static CardinalDirection GetCardinalDirectionFromAngle(float angle)
	{
		if (AngleCloseTo(angle, 0f))
			return CardinalDirection.North;
		else if (AngleCloseTo(angle, 90f))
			return CardinalDirection.East;
		else if (AngleCloseTo(angle, 180f))
			return CardinalDirection.South;
		else if (AngleCloseTo(angle, 270f))
			return CardinalDirection.West;
		else
			Debug.LogWarning($"Room has an unexpected rotation angle: {angle}");
		return CardinalDirection.North; // Default return value
	}

	static bool AngleCloseTo(float angle, float target)
	{
		return Mathf.Abs(Mathf.DeltaAngle(angle, target)) < ANGLE_TOLERANCE;
	}

	public static bool HasOpening(IRoom room, CardinalDirection direction)
	{
		// Rotate the direction based on the room's rotation
		CardinalDirection rotatedDirection = InverseRotateDirection(direction, room.Rotation);
		return rotatedDirection switch
		{
			CardinalDirection.North => room.Openings.North,
			CardinalDirection.East => room.Openings.East,
			CardinalDirection.South => room.Openings.South,
			CardinalDirection.West => room.Openings.West,
			_ => false
		};
	}

	public static CardinalDirection InverseRotateDirection(CardinalDirection direction, CardinalDirection rotation)
	{
		int directionIndex = (int)direction;
		int rotationIndex = (int)rotation;
		return (CardinalDirection)((directionIndex - rotationIndex + 4) % 4);
	}

	public static CardinalDirection GetOppositeDirection(CardinalDirection direction)
	{
		return direction switch
		{
			CardinalDirection.North => CardinalDirection.South,
			CardinalDirection.East => CardinalDirection.West,
			CardinalDirection.South => CardinalDirection.North,
			CardinalDirection.West => CardinalDirection.East,
			_ => CardinalDirection.North
		};
	}

	public static Vector3 CalculatePassagePosition(IRoom room1, IRoom room2)
	{
		Vector3 pos1 = new Vector3(room1.Position.x * RoomManager.ROOM_SIZE, 0, room1.Position.y * RoomManager.ROOM_SIZE);
		Vector3 pos2 = new Vector3(room2.Position.x * RoomManager.ROOM_SIZE, 0, room2.Position.y * RoomManager.ROOM_SIZE);
		return (pos1 + pos2) / 2f;
	}
}
