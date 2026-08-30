using UnityEngine;

public interface ISnapshotterRelay
{
	ICharacterRoot RelayedCharacter { get; set; }
}

public class SnapshotterRelay : MonoBehaviour, ISnapshotterRelay
{
	public ICharacterRoot RelayedCharacter { get; set; }
}
