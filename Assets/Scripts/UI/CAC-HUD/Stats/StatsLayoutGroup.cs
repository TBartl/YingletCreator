using System.Linq;
using UnityEngine;

public class StatsLayoutGroup : MonoBehaviour
{
	[SerializeField] GameObject _statPrefab;

	void Awake()
	{
		// Destroy all existing children
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		var resourceProvider = Singletons.GetSingleton<ICompositeResourceLoader>();
		var statIds = resourceProvider.LoadStats().OrderBy(s => s.OrderIndex).ToList();

		foreach (var statId in statIds)
		{
			using (_statPrefab.TemporarilyDisable())
			{
				var statInstance = Instantiate(_statPrefab, transform);
				statInstance.GetComponentSafe<IStatReference>().Stat = statId;
				statInstance.SetActive(true);
			}
		}
	}
}
