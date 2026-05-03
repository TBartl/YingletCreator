using Reactivity;
using System.Collections;
using UnityEngine;


/// <summary>
/// Provides a value corresponding to how much an element should follow point tracking versus its normal behaviour
/// </summary>
public interface IPointTrackingWeightProvider
{
	/// <summary>
	/// How much we should be following point tracking. 0 = not at all, 1 = fully
	/// </summary>
	float Weight { get; }
}

public class PointTrackingWeightProvider : MonoBehaviour, IPointTrackingWeightProvider
{
	[SerializeField] float _changeTime = .4f;
	[SerializeField] AnimationCurve _curve;

	Observable<float> _weight = new(0);
	private IPointTrackingLocationProvider _locationProvider;
	private Coroutine _coroutine;

	public float Weight => _weight.Val;


	private void Awake()
	{
		_locationProvider = this.GetComponentInParentSafe<IPointTrackingLocationProvider>();
		_locationProvider.Active.OnChanged += Active_OnChanged;
	}

	private void OnDestroy()
	{
		_locationProvider.Active.OnChanged -= Active_OnChanged;
	}

	private void Active_OnChanged(bool arg1, bool arg2)
	{
		this.StopAndStartCoroutine(ref _coroutine, ChangeWeightOverTime());
	}

	private void OnEnable()
	{
		Active_OnChanged(false, false); // In-case this got disabled mid weight change
	}

	IEnumerator ChangeWeightOverTime()
	{
		var from = _weight.Val;
		var to = _locationProvider.Active.Val ? 1f : 0f;
		for (float t = 0; t < _changeTime; t += Time.deltaTime)
		{
			float p = t / _changeTime;
			p = _curve.Evaluate(p);
			_weight.Val = Mathf.Lerp(from, to, p);
			yield return null;
		}
		_weight.Val = to;
	}
}