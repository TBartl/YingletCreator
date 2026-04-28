using UnityEngine;

public class LiftFogOfWarOnEnterRoom : MonoBehaviour
{
	private ICharacterRoomDetector _characterRoomDetector;
	private IFogOfWar _fogOfWar;

	private void Start()
	{
		_characterRoomDetector = this.GetCharacterRootComponent<ICharacterRoomDetector>();
		_fogOfWar = this.GetExpeditionComponent<IFogOfWar>();
		_characterRoomDetector.CurrentRoom.OnChanged += OnCharacterEnteredRoom;
	}

	private void OnDestroy()
	{
		_characterRoomDetector.CurrentRoom.OnChanged -= OnCharacterEnteredRoom;
	}

	private void OnCharacterEnteredRoom(IRoom from, IRoom to)
	{
		_fogOfWar.RevealRoom(to.Position);
	}
}
