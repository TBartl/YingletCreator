using Encounters.Runtime;
using UnityEngine;
using UnityEngine.UI;

public interface IStatusAddedBox
{
	void SetNode(IEncounterInstance encounter, AddStatusToCharacterNode node);
}

public class StatusAddedBox : MonoBehaviour, IStatusAddedBox
{
	[SerializeField] TMPro.TMP_Text _statusNameText;
	[SerializeField] Image _background;
	[SerializeField] Image _icon;
	[SerializeField] Color _negativeColor;
	[SerializeField] Color _positiveColor;

	public void SetNode(IEncounterInstance encounter, AddStatusToCharacterNode node)
	{
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
