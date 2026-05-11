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
			var characterName = encounter.FormattedCharacterName;
			return text.Replace("{CHARACTER}", characterName, System.StringComparison.OrdinalIgnoreCase);
		}
	}

}
