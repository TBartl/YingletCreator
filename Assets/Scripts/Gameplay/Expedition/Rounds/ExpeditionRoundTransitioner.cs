using Reactivity;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// While the actual transition state exists on the RoundManager, this is responsible for updating it, particularly with networking
/// </summary>
public class ExpeditionRoundTransitioner : ReactiveBehaviour
{
	[SerializeField] float _transitionInDuration = 1f;
	[SerializeField] float _incrementRoundDuration = 0.5f;
	[SerializeField] float _transitionOutDuration = 0.5f;

	private INetStateReader _netState;
	private INetEventBus _eventBus;
	private IExpeditionRoundManager _roundManager;
	private Computed<bool> _allCharactersAsleep;
	private Coroutine _coroutine;

	void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();

		_roundManager = this.GetComponentSafe<IExpeditionRoundManager>();
		_allCharactersAsleep = CreateComputed(ComputeAllCharactersAsleep);

		_eventBus.Subscribe<Message_ChangeRoundTransitionState>(OnNetMessage);
		_allCharactersAsleep.OnChanged += OnAllCharactersAsleepChanged;
	}

	private void Start()
	{
		StartTransition();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_allCharactersAsleep != null)
		{
			_allCharactersAsleep.OnChanged -= OnAllCharactersAsleepChanged;
		}
	}

	bool ComputeAllCharactersAsleep()
	{
		return _roundManager.CharacterRoundStates.All(c => c.IsAsleep.Val);
	}


	private void OnAllCharactersAsleepChanged(bool from, bool to)
	{
		if (to != true) return;
		if (_netState.IsConnectedClient) return; // Only the host should trigger the round transition
		StartTransition();
	}

	void StartTransition()
	{
		if (_coroutine != null)
		{
			Debug.LogError("Already transitioning rounds, but all characters are asleep again.");
			StopCoroutine(_coroutine);
		}
		_coroutine = StartCoroutine(TransitionRound());
	}

	IEnumerator TransitionRound()
	{
		_eventBus.SendToAll(new Message_ChangeRoundTransitionState() { State = RoundTransitionState.TransitioningIn });
		yield return new WaitForSeconds(_transitionInDuration);
		_eventBus.SendToAll(new Message_ChangeRoundTransitionState() { State = RoundTransitionState.IncrementRound });
		yield return new WaitForSeconds(_incrementRoundDuration);
		_eventBus.SendToAll(new Message_ChangeRoundTransitionState() { State = RoundTransitionState.TransitionOut });
		yield return new WaitForSeconds(_transitionOutDuration);
		_eventBus.SendToAll(new Message_ChangeRoundTransitionState() { State = RoundTransitionState.None });
		_coroutine = null;
	}

	private void OnNetMessage(Message_ChangeRoundTransitionState message, ulong senderClientId)
	{
		if (senderClientId != NetworkManager.ServerClientId) return;
		_roundManager.SetTransitionState(message.State);
	}
}

public struct Message_ChangeRoundTransitionState : INetMessage
{
	public RoundTransitionState State;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref State);
	}
}
