using Reactivity;
using System.Collections;
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

public class ExpeditionManager : MonoBehaviour, IExpeditionManager
{
	[SerializeField] GameObject _expeditionPrefab;
	[SerializeField] float _startExpeditionDuration = 1f;

	Observable<ExpeditionState> _state = new Observable<ExpeditionState>(ExpeditionState.None);
	Observable<GameObject> _rootObject = new Observable<GameObject>(null);

	public GameObject RootObject => _rootObject.Val;

	IReadOnlyObservable<ExpeditionState> IExpeditionManager.State => _state;

	public void StartExpedition()
	{
		StartCoroutine(StartExpeditionCoroutine());
	}

	IEnumerator StartExpeditionCoroutine()
	{
		_state.Val = ExpeditionState.Starting;
		yield return new WaitForSeconds(_startExpeditionDuration);
		_rootObject.Val = Instantiate(_expeditionPrefab);
		_state.Val = ExpeditionState.Running;
	}
}
