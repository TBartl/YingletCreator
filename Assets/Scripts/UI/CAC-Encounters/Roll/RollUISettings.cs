using Encounters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RollUISettings", menuName = "Scriptable Objects/Misc/Roll UI Settings")]

public class RollUISettings : ScriptableObject
{
	[SerializeField] RollClassificationColors[] _rollClassificationColors;
	private Dictionary<RollClassification, RollClassificationColors> _rollClassificationColorMap;
	public IReadOnlyDictionary<RollClassification, RollClassificationColors> RollClassificationColorMap
	{
		get
		{
			_rollClassificationColorMap ??= _rollClassificationColors.ToDictionary(x => x.Classification);
			return _rollClassificationColorMap;
		}
	}
}

[System.Serializable]
public class RollClassificationColors
{
	public RollClassification Classification;
	public Color TextColor;
	public Color BackgroundColor;
	public Color JuicyColor;
	public string Text;
}
