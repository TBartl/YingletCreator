using Reactivity;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class EyeExpressionWithTime
{
	public EyeExpression State = EyeExpression.Normal;
	public float Time = 1;
}

[CreateAssetMenu(fileName = "EyeExpressionSequence", menuName = "Scriptable Objects/Misc/Animation/EyeExpressionSequence")]
public class EyeExpressionSequence : ScriptableObject
{
	[SerializeField] EyeExpressionWithTime[] _sequence;
	public EyeExpressionWithTime[] Sequence => _sequence;

	public IEyeExpressionSequencePlayer CreatePlayer()
	{
		return new EyeExpressionSequencePlayer(this);
	}

	class EyeExpressionSequencePlayer : IEyeExpressionSequencePlayer
	{
		EyeExpressionSequence _sequence;

		Observable<bool> _isActive = new();
		Observable<EyeExpression> _expression = new Observable<EyeExpression>();
		private Coroutine _coroutine;

		public EyeExpressionSequencePlayer(EyeExpressionSequence sequence)
		{
			_sequence = sequence;
		}


		public bool IsActive => _isActive.Val;
		public EyeExpression Expression => _expression.Val;
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
				_expression.Val = state.State;
				yield return new WaitForSeconds(state.Time);
			}
			_isActive.Val = false;
		}
	}
}

public interface IEyeExpressionSequencePlayer
{
	void Play();
	void Stop();
	bool IsActive { get; }
	EyeExpression Expression { get; }
}

