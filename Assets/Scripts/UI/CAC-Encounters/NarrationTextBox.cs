using Character.Creator;
using Encounters.Runtime;
using UnityEngine;

public interface INarrationTextBox
{
	void SetNode(IEncounterInstance encounter, NarrationNode node);
}

public class NarrationTextBox : MonoBehaviour, INarrationTextBox
{
	public void SetNode(IEncounterInstance encounter, NarrationNode node)
	{
		var text = this.GetComponentInChildrenSafe<TMPro.TMP_Text>();
		var characterData = encounter.Character.GetComponentInChildrenSafe<ICustomizationDataRepository>();
		var pronouns = characterData.CustomizationData.GenderData.Pronouns.Val;
		var processedText = ProcessText(node.Text, pronouns);
		text.SetText(processedText);


		string ProcessText(string text, CharacterPronouns pronouns)
		{
			var characterName = encounter.FormattedCharacterName;
			return text
				.Replace("{CHARACTER}", characterName, System.StringComparison.OrdinalIgnoreCase)
				.ReplacePronouns(pronouns);
		}
	}

}
