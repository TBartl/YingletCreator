using Encounters.Runtime;
using UnityEngine;


public class PromptChoicesUI : MonoBehaviour
{
	[SerializeField] GameObject _choicePrefab;

	public void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var encounter = reference.EncounterInstance;
		var node = reference.Record.Node as PromptChoiceNode;

		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		for (int i = 0; i < node.Choices.Length; i++)
		{
			var choice = node.Choices[i];
			var choiceGO = Instantiate(_choicePrefab, transform);
			var choiceUI = choiceGO.GetComponentSafe<IPromptChoiceUI>();
			choiceUI.SetChoice(choice, i);
		}
	}
}
