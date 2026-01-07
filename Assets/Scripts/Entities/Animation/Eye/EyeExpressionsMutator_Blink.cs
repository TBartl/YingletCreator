using Reactivity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BlinkState
{
	Normal,
	Squint,
	Closed
}

public class EyeExpressionsMutator_Blink : MonoBehaviour, ICurrentEyeExpressionMutator
{
	[SerializeField] float _squintTime = .015f;
	[SerializeField] float _closedTime = .03f;

	private EyeGatherer _eyeGatherer;
	private IBlinkTimer _blinkTimer;

	Observable<BlinkState> _blinkState = new Observable<BlinkState>();

	static Dictionary<EyeExpression, Dictionary<BlinkState, EyeExpression>> _blinkMap = new()
	{
		{ EyeExpression.Normal, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.Normal },
			{ BlinkState.Squint, EyeExpression.Squint },
			{ BlinkState.Closed, EyeExpression.Closed }
		}},
		{ EyeExpression.Squint, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.Squint },
			{ BlinkState.Squint, EyeExpression.Squint },
			{ BlinkState.Closed, EyeExpression.Closed }
		}},
		{ EyeExpression.Closed, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.Closed },
			{ BlinkState.Squint, EyeExpression.Closed },
			{ BlinkState.Closed, EyeExpression.Closed }
		}},
		{ EyeExpression.ClosedHappy, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.ClosedHappy },
			{ BlinkState.Squint, EyeExpression.ClosedHappy },
			{ BlinkState.Closed, EyeExpression.ClosedHappy }
		}},
		{ EyeExpression.Shocked, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.Shocked },
			{ BlinkState.Squint, EyeExpression.Shocked },
			{ BlinkState.Closed, EyeExpression.ClosedEnergy}
		}},
		{ EyeExpression.Angry, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.Angry },
			{ BlinkState.Squint, EyeExpression.Angry },
			{ BlinkState.Closed, EyeExpression.ClosedEnergy }
		}},
		{ EyeExpression.Sad, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.Sad },
			{ BlinkState.Squint, EyeExpression.Sad },
			{ BlinkState.Closed, EyeExpression.Closed }
		}},
		{ EyeExpression.ClosedEnergy, new Dictionary<BlinkState, EyeExpression> {
			{ BlinkState.Normal, EyeExpression.ClosedEnergy },
			{ BlinkState.Squint, EyeExpression.ClosedEnergy },
			{ BlinkState.Closed, EyeExpression.ClosedEnergy }
		}},
	};

	void Awake()
	{
		_blinkTimer = this.GetComponent<IBlinkTimer>();

		_blinkTimer.OnBlink += BlinkTimer_OnBlink;
	}

	void OnDestroy()
	{
		_blinkTimer.OnBlink -= BlinkTimer_OnBlink;
	}

	private void BlinkTimer_OnBlink()
	{
		StartCoroutine(Blink());
	}
	IEnumerator Blink()
	{
		_blinkState.Val = BlinkState.Squint;
		yield return new WaitForSeconds(_squintTime);
		_blinkState.Val = BlinkState.Closed;
		yield return new WaitForSeconds(_closedTime);
		_blinkState.Val = BlinkState.Squint;
		yield return new WaitForSeconds(_squintTime);
		_blinkState.Val = BlinkState.Normal;
	}

	public EyeExpression Mutate(EyeExpression input)
	{
		if (!_blinkMap.TryGetValue(input, out var blinkStateMap))
		{
			Debug.LogError($"No blink state map for {input}");
			return input;
		}
		if (!blinkStateMap.TryGetValue(_blinkState.Val, out var output))
		{
			Debug.LogError($"No blink state for {input} and {_blinkState.Val}");
			return input;
		}
		return output;
	}
}
