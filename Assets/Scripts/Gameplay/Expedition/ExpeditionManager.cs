using Reactivity;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public enum ExpeditionState
{
	None,
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

	Observable<ExpeditionState> _state = new Observable<ExpeditionState>(ExpeditionState.None);
	Observable<GameObject> _rootObject = new Observable<GameObject>(null);
	private INetEventBus _eventBus;

	public GameObject RootObject => _rootObject.Val;

	IReadOnlyObservable<ExpeditionState> IExpeditionManager.State => _state;

	public void Initialize()
	{
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
		_eventBus.SendToAll(new Message_StartExpedition());
	}


	private void OnTransitionToExpedition(Message_TransitionToExpedition message, ulong senderClientId)
	{
		_state.Val = ExpeditionState.Starting;

	}
	private void OnStartExpedition(Message_StartExpedition message, ulong senderClientId)
	{
		_rootObject.Val = Instantiate(_expeditionPrefab);
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
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
	}
}