using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class RollUIModifierSpawner : MonoBehaviour
{
	void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var node = reference.Record.Node as RollNode;
		var stat = node.RollInstructions.Stat;

		// We usually want both the D6 and the stat modifier
		// Unless no stat is specified, in which case we only want the D6
		int numToLeave = stat == null ? 1 : 2;
		for (int i = transform.childCount - 1; i >= numToLeave; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		int totalChildren = transform.childCount;
		var grid = this.GetComponentSafe<GridLayoutGroup>();
		grid.constraintCount = (totalChildren + 3) / 4; // Kind of hacky to do this here but w/e
	}
}
