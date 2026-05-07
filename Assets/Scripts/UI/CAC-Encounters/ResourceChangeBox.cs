using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public interface IResourceChangeBox
{
	void SetNode(IEncounterInstance encounter, ChangeCharacterResourceNode node);
}

public class ResourceChangeBox : MonoBehaviour, IResourceChangeBox
{
	[SerializeField] TMPro.TMP_Text _topText;
	[SerializeField] TMPro.TMP_Text _bottomText;
	[SerializeField] Color _negativeColor;


	public void SetNode(IEncounterInstance encounter, ChangeCharacterResourceNode node)
	{
		var iconName = node.Resource.TextIconName;
		_topText.text = node.Delta >= 0 ? $"{iconName} Gained" : $"{iconName} Lost";
		var sign = node.Delta >= 0 ? "+" : "-";
		_bottomText.text = $"{sign}{Mathf.Abs(node.Delta)} <sprite name=\"{iconName}\">";

		if (node.Delta < 0)
		{
			this.GetComponentSafe<Image>().color = _negativeColor;
		}
	}
}
