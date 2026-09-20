using Encounters.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollUIStatModifier : Tooltip
{
	[SerializeField] public Image _icon;
	[SerializeField] public TMP_Text _modifierText;
	private RollNodeVisitData _data;

	private string _tooltipText;
	public override string Text => _tooltipText;

	void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var node = reference.Record.Node as RollNode;
		_data = reference.Record.VisitData as RollNodeVisitData;

		var stat = node.RollInstructions.Stat;

		if (stat != null)
		{
			_icon.sprite = stat.OutlinedIcon;
			_icon.color = stat.Color;
			var statNameWithArticle = TMPUtils.WithArticle($"<b>{stat.DisplayName}</b>", stat.DisplayName);
			_tooltipText = $"This is {statNameWithArticle} roll.\nContribution from that stat.";
		}

		AddReflector(Reflect);
	}

	void Reflect()
	{
		_modifierText.text = _data.StatValue.ToString();
	}
}
