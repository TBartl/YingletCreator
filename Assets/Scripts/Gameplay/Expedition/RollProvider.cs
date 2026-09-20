using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IRollProvider
{
	int RollD6();
}

internal class RollProvider : MonoBehaviour, IRollProvider, IInitializable
{
	public const int MaxRollValue = 9999;

	private IDeterministicRandomProvider _random;

	Queue<int> _forcedRolls = new Queue<int>();

	public void Initialize()
	{
		_random = this.GetComponentSafe<IDeterministicRandomProvider>();

		_forcedRolls.Enqueue(1);
		_forcedRolls.Enqueue(1);
		_forcedRolls.Enqueue(6);
		_forcedRolls.Enqueue(6);
	}
	public int RollD6()
	{
		if (_forcedRolls.Any())
		{
			var result = _forcedRolls.Dequeue();
			Debug.LogWarning($"Using forced roll of {result}. These shouldn't be set outside of recording purposes.");
			return result;
		}
		return _random.GetNextRandomInt(1, 7);
	}
}
