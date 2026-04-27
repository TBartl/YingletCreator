using UnityEngine;

public class LiftFogOfWarOnEnterRoom : MonoBehaviour
{
	private IFogOfWar _fogOfWar;

	private void Start()
	{
		_fogOfWar = this.GetExpeditionComponent<IFogOfWar>();
	}

	private void OnTriggerEnter(Collider other)
	{
		var roomTrigger = other.GetComponent<RoomTrigger>();
		if (roomTrigger != null)
		{
			var room = roomTrigger.GetComponentInParentSafe<IRoom>();
			_fogOfWar.RevealRoom(room);
		}
	}
}
