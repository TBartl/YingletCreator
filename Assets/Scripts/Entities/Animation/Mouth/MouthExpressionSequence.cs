using Reactivity;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class MouthExpressionWithTime
{
	public MouthExpression Expression = MouthExpression.Grin;
	public MouthOpenAmount OpenAmount = MouthOpenAmount.Closed;
	public float Time = 1;
}

[CreateAssetMenu(fileName = "MouthExpressionSequence", menuName = "Scriptable Objects/Misc/Animation/MouthExpressionSequence")]
public class MouthExpressionSequence : ScriptableObject
{
	[SerializeField] MouthExpressionWithTime[] _sequence;
	public MouthExpressionWithTime[] Sequence => _sequence;

	public IMouthExpressionSequencePlayer CreatePlayer()
	{
		return new MouthExpressionSequencePlayer(this);
	}

	class MouthExpressionSequencePlayer : IMouthExpressionSequencePlayer
	{
		MouthExpressionSequence _sequence;

		Observable<bool> _isActive = new();
		Observable<MouthExpression> _expression = new Observable<MouthExpression>();
		Observable<MouthOpenAmount> _openAmount = new Observable<MouthOpenAmount>();
		private Coroutine _coroutine;

		public MouthExpressionSequencePlayer(MouthExpressionSequence sequence)
		{
			_sequence = sequence;
		}

		public bool IsActive => _isActive.Val;
		public MouthExpression Expression => _expression.Val;
		public MouthOpenAmount OpenAmount => _openAmount.Val;

		public void Play()
		{
			CoroutineRunner.S.StopAndStartCoroutine(ref _coroutine, PlaySequence());
		}

		public void Stop()
		{
			CoroutineRunner.S.StopCoroutineIfRunning(ref _coroutine);
			_isActive.Val = false;
		}

		IEnumerator PlaySequence()
		{
			_isActive.Val = true;
			foreach (var state in _sequence.Sequence)
			{
				_expression.Val = state.Expression;
				_openAmount.Val = state.OpenAmount;
				yield return new WaitForSeconds(state.Time);
			}
			_isActive.Val = false;
		}
	}
}

public interface IMouthExpressionSequencePlayer
{
	void Play();
	void Stop();
	bool IsActive { get; }
	MouthExpression Expression { get; }
	MouthOpenAmount OpenAmount { get; }
}
