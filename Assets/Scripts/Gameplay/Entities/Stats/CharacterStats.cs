using Reactivity;
using System.Collections.Generic;

public interface ICharacterStats
{
	int GetStat(StatId statId);
}

public class CharacterStats : ReactiveBehaviour, ICharacterStats, IInitializable
{
	private IClassReference _classReference;
	Computed<Dictionary<StatId, int>> _stats;

	public void Initialize()
	{
		// Might make these providers eventually, but with only 2 expected sources maybe I just do it here
		_classReference = this.GetCharacterRootComponent<IClassReference>();

		_stats = CreateComputed(ComputeStats);
	}

	private Dictionary<StatId, int> ComputeStats()
	{
		var dict = new Dictionary<StatId, int>();

		var classStatsArray = _classReference.Class.Stats;
		foreach (var stat in classStatsArray)
		{
			dict[stat.Stat.LoadSync()] = stat.Value;
		}

		return dict;
	}

	public int GetStat(StatId statId)
	{
		if (_stats.Val.TryGetValue(statId, out var statValue))
		{
			return statValue;
		}
		return 0;
	}
}
