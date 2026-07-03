using UnityEngine;

public static class TMPUtils
{
	public static Color TextRed = new Color(1, 0.3529f, 0.3451f);
	public static Color TextGreen = new Color(0.3216f, 0.9059f, 0.3137f);

	public const string TooltipRed = "#FF5A58";
	public const string TooltipGreen = "#52E750";
	public const string FlavorStart = "<color=#FFFFFF99><size=65%>";
	public const string FlavorEnd = "</size></color>";
	public const string EnergySprite = "<sprite tint=\"1\" name=\"Energy\">";
	public const string DiceSprite = "<sprite tint=\"1\" name=\"Dice\">";

	public static string ColorizeNumber(int number, int baseline = 0)
	{
		int delta = number - baseline;
		if (delta > 0)
		{
			return $"<color={TooltipGreen}>{number}</color>";
		}
		else if (delta < 0)
		{
			return $"<color={TooltipRed}>{number}</color>";
		}
		else
		{
			return number.ToString();
		}
	}

	public static string ColorizeLabelWithNumber(string formattedLabel, int number)
	{
		string prefixedNumber = number > 0 ? $"+{number}" : number.ToString();
		string label = string.Format(formattedLabel, prefixedNumber);
		if (number > 0)
		{
			return $"<color={TooltipGreen}>{label}</color>";
		}
		else if (number < 0)
		{
			return $"<color={TooltipRed}>{label}</color>";
		}
		else
		{
			return label;
		}
	}

}
