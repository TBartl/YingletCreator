using Networking;
using Reactivity;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public enum ExpeditionState
{
	Planning,
	Starting,
	Running,
}

public interface IExpeditionManager
{
	IReadOnlyObservable<ExpeditionState> State { get; }
	void StartExpedition();
	GameObject RootObject { get; }
}

public class ExpeditionManager : MonoBehaviour, IExpeditionManager, IInitializable
{
	[SerializeField] GameObject _expeditionPrefab;
	[SerializeField] float _startExpeditionDuration = 1f;

	Observable<ExpeditionState> _state = new Observable<ExpeditionState>(ExpeditionState.Planning);
	Observable<GameObject> _rootObject = new Observable<GameObject>(null);
	private INetIdentityProvider _netIdentityProvider;
	private INetEventBus _eventBus;

	public GameObject RootObject => _rootObject.Val;

	IReadOnlyObservable<ExpeditionState> IExpeditionManager.State => _state;

	public void Initialize()
	{
		_netIdentityProvider = Singletons.GetSingleton<INetIdentityProvider>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_eventBus.Subscribe<Message_TransitionToExpedition>(OnTransitionToExpedition);
		_eventBus.Subscribe<Message_StartExpedition>(OnStartExpedition);
	}

	private void OnDestroy()
	{
		_eventBus?.Unsubscribe<Message_TransitionToExpedition>(OnTransitionToExpedition);
		_eventBus?.Unsubscribe<Message_StartExpedition>(OnStartExpedition);
	}


	public void StartExpedition()
	{
		_eventBus.SendToAll(new Message_TransitionToExpedition());
		StartCoroutine(StartExpeditionCoroutine());
	}

	IEnumerator StartExpeditionCoroutine()
	{
		yield return new WaitForSeconds(_startExpeditionDuration);
		_eventBus.SendToAll(new Message_StartExpedition(_netIdentityProvider.GetNextId()));
	}


	private void OnTransitionToExpedition(Message_TransitionToExpedition message, ulong senderClientId)
	{
		_state.Val = ExpeditionState.Starting;

	}
	private void OnStartExpedition(Message_StartExpedition message, ulong senderClientId)
	{
		_netIdentityProvider.ForceNextId(message.NetIdToSync);
		using (var disabler = _expeditionPrefab.TemporarilyDisable())
		{
			_rootObject.Val = Instantiate(_expeditionPrefab);
			_rootObject.Val.GetComponentInChildrenSafe<IDeterministicRandomProvider>().SetSeed(message.Seed);
		}

		_state.Val = ExpeditionState.Running;
	}
}

struct Message_TransitionToExpedition : INetMessage
{
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
	}
}

struct Message_StartExpedition : INetMessage
{
	public int Seed;
	public ulong NetIdToSync;
	public Message_StartExpedition(ulong netIdToSync)
	{
		Seed = Random.Range(int.MinValue, int.MaxValue);
		NetIdToSync = netIdToSync;
	}
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetIdToSync);
	}
}