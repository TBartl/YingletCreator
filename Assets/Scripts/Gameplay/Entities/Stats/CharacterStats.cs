using Reactivity;
using System.Collections.Generic;

public interface ICharacterStats
{
	int GetStat(StatId statId);
	IEnumerable<CharacterStatRecord> GetStatRecords(StatId statId);
}

/// <summary>
/// Store stats as a list of records so we can display their breakdown later
/// </summary>
public class CharacterStatRecord
{
	public string Source;
	public int Delta;
}

public class CharacterStats : ReactiveBehaviour, ICharacterStats, IInitializable
{
	private ICompositeResourceLoader _resourceProvider;
	private IClassReference _classReference;
	private ICharacterStatuses _characterStatuses;
	Dictionary<StatId, Computed<IEnumerable<CharacterStatRecord>>> _statRecords = new();
	Dictionary<StatId, Computed<int>> _stats = new();

	public void Initialize()
	{
		_resourceProvider = Singletons.GetSingleton<ICompositeResourceLoader>();

		// Might make these providers eventually, but with only 2 expected sources maybe I just do it here
		_classReference = this.GetCharacterRootComponent<IClassReference>();
		_characterStatuses = this.GetCharacterRootComponent<ICharacterStatuses>();


		var stats = _resourceProvider.LoadStats();
		foreach (var stat in stats)
		{
			_statRecords[stat] = ComputeStatRecord(stat);
			_stats[stat] = ComputeStat(stat);
		}
	}

	Computed<IEnumerable<CharacterStatRecord>> ComputeStatRecord(StatId stat)
	{
		return this.CreateComputed<IEnumerable<CharacterStatRecord>>(() =>
		{
			var records = new List<CharacterStatRecord>();
			var classStatsArray = _classReference.Class.Stats;
			foreach (var classStat in classStatsArray)
			{
				if (classStat.Stat.LoadSync() == stat)
				{
					records.Add(new CharacterStatRecord
					{
						Source = "Class",
						Delta = classStat.Value
					});
				}
			}
			foreach (var status in _characterStatuses.Statuses)
			{
				var statusEffects = status.StatusEffects;
				foreach (var statusEffect in statusEffects)
				{
					if (statusEffect is StatusEffect_ChangeStat changeStat && changeStat.Stat == stat)
					{
						records.Add(new CharacterStatRecord
						{
							Source = status.DisplayName,
							Delta = changeStat.Delta
						});
					}
				}
			}
			return records;
		});
	}

	private Computed<int> ComputeStat(StatId stat)
	{
		return this.CreateComputed<int>(() =>
		{
			int total = 0;
			foreach (var record in _statRecords[stat].Val)
			{
				total += record.Delta;
			}
			return total;
		});
	}

	public IEnumerable<CharacterStatRecord> GetStatRecords(StatId statId)
	{
		return _statRecords[statId].Val;
	}

	public int GetStat(StatId statId)
	{
		return _stats[statId].Val;
	}
}
