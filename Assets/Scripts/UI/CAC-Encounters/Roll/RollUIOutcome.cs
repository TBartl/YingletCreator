using Encounters.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IRollUIOutcome
{
	void SetResult(RollBlockNode branch);
}

public class RollUIOutcome : MonoBehaviour, IRollUIOutcome
{
	[SerializeField] TMP_Text _numberText;
	[SerializeField] TMP_Text _descriptionText;
	[SerializeField] Image _background;

	[SerializeField] RollUISettings _settings;

	public void SetResult(RollBlockNode branch)
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var node = reference.Record.Node as RollNode;

		var settings = _settings.RollClassificationColorMap[branch.Classification];
		int minValue = CalculateMinValue(node, branch);

		_numberText.text = GetNumberText(minValue, branch.MaxValueInclusive);
		_numberText.color = settings.TextColor;

		_descriptionText.text = settings.Text;
		_descriptionText.color = settings.TextColor;

		_background.color = settings.BackgroundColor;
	}

	private int CalculateMinValue(RollNode node, RollBlockNode myBranch)
	{
		int minValue = 0;
		var branches = node.Branches;
		foreach (var branch in node.Branches)
		{
			if (branch == myBranch) break;
			minValue = branch.MaxValueInclusive + 1;
		}
		return minValue;
	}

	private string GetNumberText(int minValue, int maxValue)
	{
		if (minValue == maxValue)
		{
			return minValue.ToString();
		}
		else if (maxValue >= RollProvider.MaxRollValue)
		{
			return $"{minValue}+";
		}
		return $"{minValue}-{maxValue}";
	}
}
