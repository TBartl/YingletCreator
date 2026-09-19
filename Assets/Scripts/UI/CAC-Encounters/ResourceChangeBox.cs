using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class ResourceChangeBox : MonoBehaviour
{
	[SerializeField] TMPro.TMP_Text _topText;
	[SerializeField] TMPro.TMP_Text _bottomText;
	[SerializeField] Color _negativeColor;


	public void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var node = reference.Record.Node as ChangeCharacterResourceNode;

		var displayName = node.Resource.TextDisplayName;
		_topText.text = node.Delta >= 0 ? $"{displayName} Gained" : $"{displayName} Lost";
		var sign = node.Delta >= 0 ? "+" : "-";
		_bottomText.text = $"{sign}{Mathf.Abs(node.Delta)} {node.Resource.TMPIcon}";

		if (node.Delta < 0)
		{
			this.GetComponentSafe<Image>().color = _negativeColor;
		}
	}
}
