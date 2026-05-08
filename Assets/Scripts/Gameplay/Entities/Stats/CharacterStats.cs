using Reactivity;
using System.Collections.Generic;

public interface ICharacterStats
{
	int GetStat(StatId statId);
}

public class CharacterStats : ReactiveBehaviour, ICharacterStats, IInitializable
{
	private IClassReference _classReference;
	private ICharacterStatuses _characterStatuses;
	Computed<Dictionary<StatId, int>> _stats;

	public void Initialize()
	{
		// Might make these providers eventually, but with only 2 expected sources maybe I just do it here
		_classReference = this.GetCharacterRootComponent<IClassReference>();
		_characterStatuses = this.GetCharacterRootComponent<ICharacterStatuses>();

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

		foreach (var status in _characterStatuses.Statuses)
		{
			var statusEffects = status.StatusEffects;
			foreach (var statusEffect in statusEffects)
			{
				if (statusEffect is StatusEffect_ChangeStat changeStat)
				{
					if (dict.TryGetValue(changeStat.Stat, out var currentValue))
					{
						dict[changeStat.Stat] = currentValue + changeStat.Delta;
					}
					else
					{
						dict[changeStat.Stat] = changeStat.Delta;
					}
				}
			}
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
