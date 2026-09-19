using Character.Creator;
using Encounters.Runtime;
using UnityEngine;

public class NarrationTextBox : MonoBehaviour
{

	void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var encounter = reference.EncounterInstance;
		var node = reference.Record.Node as NarrationNode;

		var text = this.GetComponentInChildrenSafe<TMPro.TMP_Text>();
		var characterData = encounter.Character.GetComponentInChildrenSafe<ICustomizationDataRepository>();
		var processedText = ProcessText(node.Text, characterData.CustomizationData.GenderData);
		text.SetText(processedText);


		string ProcessText(string text, ObservableCustomizationGenderData genderData)
		{
			var characterName = encounter.FormattedCharacterName;
			return text
				.Replace("{CHARACTER}", characterName, System.StringComparison.OrdinalIgnoreCase)
				.ReplacePronouns(genderData);
		}
	}
}
