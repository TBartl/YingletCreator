using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class StatusAddedBox : MonoBehaviour
{
	[SerializeField] TMPro.TMP_Text _statusNameText;
	[SerializeField] Image _background;
	[SerializeField] Image _icon;
	[SerializeField] Color _negativeColor;
	[SerializeField] Color _positiveColor;

	public void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var node = reference.Record.Node as AddStatusToCharacterNode;

		var status = node.Status;
		_statusNameText.text = status.DisplayName;

		if (status.Sentiment == StatusSentiment.Negative)
		{
			_background.color = _negativeColor;
		}
		else if (status.Sentiment == StatusSentiment.Positive)
		{
			_background.color = _positiveColor;
		}

		_icon.sprite = status.Icon;
	}
}
