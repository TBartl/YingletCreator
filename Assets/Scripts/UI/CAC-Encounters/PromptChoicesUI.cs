using Encounters.Runtime;
using UnityEngine;

public interface IPromptChoicesUI
{
	void SetNode(IEncounterInstance encounter, PromptChoiceNode node);
}

public class PromptChoicesUI : MonoBehaviour, IPromptChoicesUI
{
	[SerializeField] GameObject _choicePrefab;

	public void SetNode(IEncounterInstance encounter, PromptChoiceNode node)
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		foreach (var choice in node.Choices)
		{
			var choiceGO = Instantiate(_choicePrefab, transform);
			var choiceUI = choiceGO.GetComponent<IPromptChoiceUI>();
			choiceUI.SetChoice(encounter, choice);
		}
	}
}
