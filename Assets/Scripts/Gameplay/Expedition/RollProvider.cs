using Encounters;
using UnityEngine;

public interface IRollProvider
{
	int GetRoll(ICharacterRoot character, RollType rollType);
}

internal class RollProvider : MonoBehaviour, IRollProvider, IInitializable
{
	private IDeterministicRandomProvider _random;

	public void Initialize()
	{
		_random = this.GetComponentSafe<IDeterministicRandomProvider>();
	}
	public int GetRoll(ICharacterRoot character, RollType rollType)
	{
		int sum = 0;

		// Basic betrayal at house on the hill like rolling assuming stat of 4
		for (int die = 0; die < 4; die++)
		{
			sum += _random.GetNextRandomInt(0, 3);
		}

		return sum;
	}
}
