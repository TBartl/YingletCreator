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
}

public class ExpeditionManager : MonoBehaviour, IExpeditionManager
{
	[SerializeField] float _startExpeditionDuration = 1f;

	Observable<ExpeditionState> _state = new Observable<ExpeditionState>(ExpeditionState.None);
	IReadOnlyObservable<ExpeditionState> IExpeditionManager.State => _state;

	public void StartExpedition()
	{
		StartCoroutine(StartExpeditionCoroutine());
	}

	IEnumerator StartExpeditionCoroutine()
	{
		_state.Val = ExpeditionState.Starting;
		yield return new WaitForSeconds(_startExpeditionDuration);
		_state.Val = ExpeditionState.Running;
	}
}
