using Networking;
using Reactivity;
using Unity.Netcode;
using UnityEngine;

public interface ICharacterRoundState
{
	IReadOnlyObservable<bool> IsAsleep { get; }

	void GoToSleep();

	/// <summary>
	/// Called when the player wants to put this character back awake
	/// Otherwise, the character will automatically wake up at the start of the next round
	/// </summary>
	void WakeBackUp();
}

public class CharacterRoundState : MonoBehaviour, ICharacterRoundState
{
	private Observable<bool> _isAsleep = new Observable<bool>(false);
	private INetEventBus _eventBus;
	private IExpeditionRoundManager _roundManager;
	private ICharacterEncounterReference _encounterReference;
	private INetIdentity _netIdentity;

	public IReadOnlyObservable<bool> IsAsleep => _isAsleep;

	public void Start()
	{
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_roundManager = this.GetExpeditionComponent<IExpeditionRoundManager>();
		_encounterReference = this.GetCharacterRootComponent<ICharacterEncounterReference>();
		_netIdentity = this.GetCharacterRootComponent<INetIdentity>();

		_roundManager.CurrentRound.OnChanged += OnRoundChanged;

		_eventBus.Subscribe<Message_CharacterGoToSleep>(OnMessage_CharacterGoToSleep);
		_eventBus.Subscribe<Message_WakeBackUp>(OnMessage_WakeBackUp);
	}

	private void OnDestroy()
	{
		if (_roundManager != null)
		{
			_roundManager.CurrentRound.OnChanged -= OnRoundChanged;
		}

		_eventBus?.Unsubscribe<Message_CharacterGoToSleep>(OnMessage_CharacterGoToSleep);
		_eventBus?.Unsubscribe<Message_WakeBackUp>(OnMessage_WakeBackUp);
	}

	private void OnRoundChanged(int from, int to)
	{
		// Wake up when the round changes
		_isAsleep.Val = false;
	}

	public void GoToSleep()
	{
		_eventBus.SendToAll(new Message_CharacterGoToSleep(_netIdentity.NetId));
	}

	public void WakeBackUp()
	{
		_eventBus.SendToAll(new Message_WakeBackUp(_netIdentity.NetId));
	}

	private void OnMessage_CharacterGoToSleep(Message_CharacterGoToSleep message, ulong senderClientId)
	{
		if (message.CharacterNetId != _netIdentity.NetId) return;
		if (_encounterReference.Encounter.Val != null) return;
		_isAsleep.Val = true;
	}

	private void OnMessage_WakeBackUp(Message_WakeBackUp message, ulong senderClientId)
	{
		if (message.CharacterNetId != _netIdentity.NetId) return;
		_isAsleep.Val = false;
	}
}

struct Message_CharacterGoToSleep : INetMessage
{
	public ulong CharacterNetId;

	public Message_CharacterGoToSleep(ulong characterNetId)
	{
		CharacterNetId = characterNetId;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref CharacterNetId);
	}
}

struct Message_WakeBackUp : INetMessage
{
	public ulong CharacterNetId;

	public Message_WakeBackUp(ulong characterNetId)
	{
		CharacterNetId = characterNetId;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref CharacterNetId);
	}
}
