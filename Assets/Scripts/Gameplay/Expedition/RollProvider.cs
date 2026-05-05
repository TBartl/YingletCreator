using Encounters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IRollProvider
{
	int GetRoll(ICharacterRoot character, RollType rollType);
}

internal class RollProvider : MonoBehaviour, IRollProvider, IInitializable
{
	private IDeterministicRandomProvider _random;

	Queue<int> _forcedRolls = new Queue<int>();

	public void Initialize()
	{
		_random = this.GetComponentSafe<IDeterministicRandomProvider>();

		_forcedRolls.Enqueue(2);
		_forcedRolls.Enqueue(5);
	}
	public int GetRoll(ICharacterRoot character, RollType rollType)
	{
		if (_forcedRolls.Any())
		{
			var result = _forcedRolls.Dequeue();
			Debug.LogWarning($"Using forced roll of {result}. These shouldn't be set outside of recording purposes.");
			return result;
		}

		int sum = 0;

		// Basic betrayal at house on the hill like rolling assuming stat of 4
		for (int die = 0; die < 4; die++)
		{
			sum += _random.GetNextRandomInt(0, 3);
		}

		return sum;
	}
}
