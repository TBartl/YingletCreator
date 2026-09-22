using Encounters.Runtime;
using UnityEngine;

public class RollUIScaleWithRoll : MonoBehaviour
{
	[SerializeField] SharedEaseSettings _expandEaseSettings;
	[SerializeField] SharedEaseSettings _contractEaseSettings;
	[SerializeField] float _scale;
	private RollNodeVisitData _data;
	private Coroutine _coroutine;

	private void Start()
	{

		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;
		_data.StateObservable.OnChanged += OnRollStateChanged;
	}
	private void OnDestroy()
	{
		if (_data != null)
		{
			_data.StateObservable.OnChanged -= OnRollStateChanged;
		}
	}

	private void OnRollStateChanged(RollState from, RollState to)
	{
		if (to == RollState.Rolling)
		{
			this.StartEaseCoroutine(ref _coroutine, _expandEaseSettings, Apply);
		}
		else if (to == RollState.ShowingResult)
		{
			this.StartEaseCoroutine(ref _coroutine, _contractEaseSettings, Apply, OnComplete);
		}

		void Apply(float p)
		{
			this.transform.localScale = Vector3.one * Mathf.Lerp(1f, _scale, p);
		}
		void OnComplete()
		{
			this.transform.localScale = Vector3.one;
		}
	}
}
