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

		for (int i = 0; i < node.Choices.Length; i++)
		{
			var choice = node.Choices[i];
			var choiceGO = Instantiate(_choicePrefab, transform);
			var choiceUI = choiceGO.GetComponent<IPromptChoiceUI>();
			choiceUI.SetChoice(encounter, choice, i);
		}
	}
}
