using Reactivity;
using System.Collections;
using UnityEngine;

public class PupilOffsetMutator_Wobble : ReactiveBehaviour, IPupilOffsetMutator
{
	[SerializeField] float _maxWobbleX;
	[SerializeField] float _maxWobbleY;
	[SerializeField] Vector2 _wobbleTimeRange;
	[SerializeField] EaseSettings _wobbleEaseSettings;
	[SerializeField] float _lookAtWeight = .3f;

	IEyeControl _eyeControl;
	IPointTrackingWeightProvider _weightProvider;

	Vector2 _wobbleAmount;

	Coroutine _moveEye;

	private void Awake()
	{
		_eyeControl = this.GetComponent<IEyeControl>();
		_weightProvider = this.GetComponentInParent<IPointTrackingWeightProvider>();
	}

	public PupilOffsets Mutate(PupilOffsets input)
	{
		if (!_eyeControl.IdleEyeMovementEnabled) return input;

		float weight = Mathf.Lerp(1, _lookAtWeight, _weightProvider.Weight);
		return input.ShiftBothBy(_wobbleAmount * weight);
	}

	void OnEnable()
	{
		// Must be OnEnable instead of Start since the coroutine stops when this gets disabled and re-enabled
		StartCoroutine(Wobble());
	}

	IEnumerator Wobble()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(_wobbleTimeRange.x, _wobbleTimeRange.y));
			var targetWobbleAmount = new Vector2(Random.Range(-_maxWobbleX, _maxWobbleX), Random.Range(-_maxWobbleY, _maxWobbleY));
			// Note: _wobbleAmount probably should have been cached here for use in the function, but w/e
			this.StartEaseCoroutine(ref _moveEye, _wobbleEaseSettings, p => _wobbleAmount = Vector2.Lerp(_wobbleAmount, targetWobbleAmount, p));
		}
	}
}
