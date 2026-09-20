using Encounters.Runtime;
using UnityEngine;

public class RollUIOutcomepawner : MonoBehaviour
{
	[SerializeField] GameObject _resultPrefab;

	void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var node = reference.Record.Node as RollNode;

		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		foreach (var branch in node.Branches)
		{
			var go = Instantiate(_resultPrefab, transform);
			go.GetComponentSafe<IRollUIOutcome>().SetResult(branch);
		}
	}
}
