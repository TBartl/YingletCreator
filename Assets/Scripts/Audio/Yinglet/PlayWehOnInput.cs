using Character.Creator;
using Networking;
using Unity.Netcode;
using UnityEngine;

public class PlayWehOnInput : MonoBehaviour
{
	[SerializeField] private SoundEffectBase _soundEffect;

	private IAudioPlayer _audioPlayer;
	private ICharacterIdentity _playerIdentity;
	private ICustomizationDataRepository _dataRepo;
	private INetEventBus _eventBus;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_playerIdentity = this.GetCharacterRootComponent<ICharacterIdentity>();
		_dataRepo = this.GetCharacterRootComponent<ICustomizationDataRepository>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();

		_eventBus.Subscribe<Message_PlayWehSound>(OnMessagePlayWehSound);
	}

	private void OnDestroy()
	{
		_eventBus.Unsubscribe<Message_PlayWehSound>(OnMessagePlayWehSound);
	}

	float Shift => _dataRepo.CustomizationData.GenderData.VoicePitchShift.Val;

	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.V)) return;
		if (!_playerIdentity.IsActiveAndMine) return;

		var options = new AudioPlayOptions { Position = transform.position, PitchShift = Shift };

		// Play immediately on the client
		_audioPlayer.Play(_soundEffect, options);

		// Send message to other clients
		_eventBus.SendToAll(new Message_PlayWehSound(_playerIdentity.NetId, transform.position));
	}

	private void OnMessagePlayWehSound(Message_PlayWehSound message, ulong senderClientId)
	{
		if (_playerIdentity.IsMine) return; // We already played it, return
		if (senderClientId != _playerIdentity.OwnerClientId) return; // Not from the owner, return
		if (message.NetId != _playerIdentity.NetId) return; // Not for this character, return

		// Play the sound on this client with the received pitch shift
		var options = new AudioPlayOptions { Position = message.Position, PitchShift = Shift };
		_audioPlayer.Play(_soundEffect, options);
	}
}

public struct Message_PlayWehSound : INetMessage
{
	public ulong NetId;
	public Vector3 Position;

	public Message_PlayWehSound(ulong netId, Vector3 position)
	{
		NetId = netId;
		Position = position;
	}

	public NetworkDelivery DeliveryMethod => NetworkDelivery.Reliable;
	public bool SendToSelf => false;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
		serializer.SerializeValue(ref Position);
	}
}
