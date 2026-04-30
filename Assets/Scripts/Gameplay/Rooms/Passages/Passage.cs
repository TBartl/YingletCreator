using UnityEngine;

public interface IPassage
{
	IRoom RoomA { get; }
	IRoom RoomB { get; }

	void SetRooms(IRoom roomA, IRoom roomB);
}


public class Passage : MonoBehaviour, IPassage
{
	public IRoom RoomA { get; private set; }
	public IRoom RoomB { get; private set; }

	public void SetRooms(IRoom roomA, IRoom roomB)
	{
		RoomA = roomA;
		RoomB = roomB;
	}
}