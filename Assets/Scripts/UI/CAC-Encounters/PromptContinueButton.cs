using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class PromptContinueButton : MonoBehaviour
{
	private IActiveEncounterProvider _encounterProvider;
	private Button _button;

	void Start()
	{
		_encounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();
		_button = GetComponent<Button>();

		_button.onClick.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		if (_button == null) return;
		_button.onClick.RemoveListener(OnClick);
	}

	private void OnClick()
	{
		var encounter = _encounterProvider.ActiveEncounter.Val;
		if (encounter == null) return;
		var currentNode = encounter.CurrentNode.Val;
		if (currentNode == null) return;
		var nodeAsContinueNode = currentNode as PromptContinueNode;
		if (nodeAsContinueNode == null)
		{
			Debug.LogError("PromptContinueButton was clicked, but the current node is not a PromptContinueNode.");
			return;
		}
		nodeAsContinueNode.Continue(encounter);
	}
}
