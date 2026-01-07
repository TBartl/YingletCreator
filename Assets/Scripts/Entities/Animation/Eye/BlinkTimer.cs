using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IBlinkTimer
{
	event Action OnBlink;
}

public class BlinkTimer : MonoBehaviour, IBlinkTimer
{
	[SerializeField] Vector2 _blinkTimeRange; // Humans blink between 2 and 4
	private IEyeControl _eyeControl;
	private IEyeExpressions _eyeExpressions;
	public event Action OnBlink = delegate { };

	static EyeExpression[] IGNORE_EXPRESSIONS = new EyeExpression[]
	{
		EyeExpression.Closed,
		EyeExpression.ClosedHappy,
		EyeExpression.ClosedEnergy,
	};

	void Awake()
	{
		_eyeControl = this.GetComponent<IEyeControl>();
		_eyeExpressions = this.GetComponent<IEyeExpressions>();
	}

	void OnEnable()
	{
		// Must be OnEnable instead of Start since the coroutine stops when this gets disabled and re-enabled
		StartCoroutine(RepeatedBlinks());
	}

	IEnumerator RepeatedBlinks()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(_blinkTimeRange.x, _blinkTimeRange.y));

			// Don't blink if idle eye movement is disabled
			if (!_eyeControl.IdleEyeMovementEnabled) continue;

			// Don't blink antenna if the eye is already closed
			if (IGNORE_EXPRESSIONS.Contains(_eyeExpressions.BaseExpression)) continue;
			OnBlink();
		}
	}
}
