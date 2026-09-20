using UnityEngine;

/// <summary>
/// Root roll node UI interface
/// Mantains state information for other components
/// </summary>
public interface IRollUI
{
}

public class RollUI : MonoBehaviour, IRollUI
{
	//	[SerializeField] TMP_Text _rollNumberText;
	//	[SerializeField] TMP_Text _rollTypeText;
	//	[SerializeField] TMP_Text _rollClassificationText;

	//	public void SetNode(IEncounterInstance encounter, RollNode node, RollBlockNode blockNode, object nodeResultData)
	//	{
	//		var rollResult = (int)nodeResultData;

	//		_rollNumberText.text = rollResult.ToString();
	//		string rollType = node.RollInstructionsName;
	//		int diceCount = RollProvider.GetNumDiceToRoll(encounter.Character, node.RollInstructions);
	//		_rollTypeText.text = $"ROLL: {rollType} ({diceCount}x{TMPUtils.DiceSprite})";

	//		var classification = blockNode.Classification;
	//		_rollClassificationText.text = classification switch
	//		{
	//			Encounters.RollClassification.CriticalFailure => MakeCritical("FAILURE"),
	//			Encounters.RollClassification.Failure => "FAILURE",
	//			Encounters.RollClassification.Neutral => "NEUTRAL",
	//			Encounters.RollClassification.Success => "SUCCESS",
	//			Encounters.RollClassification.CriticalSuccess => MakeCritical("SUCCESS"),
	//			_ => throw new System.ArgumentOutOfRangeException()
	//		};

	//		if (classification == Encounters.RollClassification.CriticalFailure || classification == Encounters.RollClassification.Failure)
	//		{
	//			_rollClassificationText.color = TMPUtils.TextRed;
	//		}
	//		else if (classification == Encounters.RollClassification.Success || classification == Encounters.RollClassification.CriticalSuccess)
	//		{
	//			_rollClassificationText.color = TMPUtils.TextGreen;
	//		}

	//	}
	//	string MakeCritical(string text)
	//	{
	//		return $"<size=25%>CRITICAL</size>\n<size=75%>{text}</size>";
	//	}
}
