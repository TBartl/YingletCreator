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
		var processedText = ProcessText(node.Text);
		text.SetText(processedText);


		string ProcessText(string text)
		{
			var characterName = encounter.CharacterName;
			var formattedCharacterName = $"<b><color=#D55F4E>{characterName}</color></b>";
			return text.Replace("{CHARACTER}", formattedCharacterName, System.StringComparison.OrdinalIgnoreCase);
		}
	}

}
