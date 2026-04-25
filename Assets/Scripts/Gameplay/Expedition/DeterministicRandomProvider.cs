using UnityEngine;

public interface IDeterministicRandomProvider
{
	void SetSeed(int seed);
	int GetNextRandomInt(int minInclusive, int maxExclusive);
}

public class DeterministicRandomProvider : MonoBehaviour, IDeterministicRandomProvider
{
	private System.Random _random;

	public void SetSeed(int seed)
	{
		_random = new System.Random(seed);
	}

	public int GetNextRandomInt(int minInclusive, int maxExclusive)
	{
		return _random.Next(minInclusive, maxExclusive);
	}
}
