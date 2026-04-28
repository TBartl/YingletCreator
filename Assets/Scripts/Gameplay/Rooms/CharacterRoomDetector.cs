using Networking;
using Reactivity;
using Unity.Netcode;
using UnityEngine;


public interface ICharacterRoomDetector
{
	IReadOnlyObservable<IRoom> CurrentRoom { get; }
}

internal class CharacterRoomDetector : MonoBehaviour, ICharacterRoomDetector, IInitializable
{
	private ICharacterIdentity _identity;
	private IRoomManager _roomManager;
	private INetEventBus _eventBus;

	Observable<IRoom> _currentRoom = new Observable<IRoom>(null);
	public IReadOnlyObservable<IRoom> CurrentRoom => _currentRoom;

	public void Initialize()
	{
		_identity = this.GetComponentInParentSafe<ICharacterIdentity>();
		_roomManager = this.GetExpeditionComponent<IRoomManager>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_eventBus.Subscribe<Message_CharacterEnteredRoom>(OnMessage_CharacterEnteredRoom);
		_currentRoom.Val = _roomManager.GetRoom(Vector2Int.zero);
	}

	private void OnDestroy()
	{
		_eventBus?.Unsubscribe<Message_CharacterEnteredRoom>(OnMessage_CharacterEnteredRoom);
	}

	private void OnMessage_CharacterEnteredRoom(Message_CharacterEnteredRoom message, ulong senderClientId)
	{
		if (message.NetId != _identity.NetId) return;
		var room = _roomManager.GetRoom(message.RoomPosition);
		_currentRoom.Val = room;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_identity.IsMine) return;
		var roomTrigger = other.GetComponent<RoomTrigger>();
		if (roomTrigger != null)
		{
			var room = roomTrigger.GetComponentInParentSafe<IRoom>();
			_eventBus.SendToAll(new Message_CharacterEnteredRoom(_identity.NetId, room.Position));
		}
	}
}


struct Message_CharacterEnteredRoom : INetMessage
{
	public ulong NetId;
	public Vector2Int RoomPosition;
	public Message_CharacterEnteredRoom(ulong netId, Vector2Int roomPosition)
	{
		NetId = netId;
		RoomPosition = roomPosition;
	}
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
		serializer.SerializeValue(ref RoomPosition);
	}
}