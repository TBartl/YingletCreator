using UnityEngine;

public interface IRoom
{
	Vector2Int Position { get; }

}

public class Room : MonoBehaviour, IRoom
{
	// Hardcoded for now - to be revised later
	[SerializeField] private Vector2Int _position;
	public Vector2Int Position => _position;
}
