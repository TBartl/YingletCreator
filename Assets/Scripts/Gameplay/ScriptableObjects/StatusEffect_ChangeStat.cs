using System.Text;
using UnityEngine;


/// <summary>
/// Like a component for StatusId
/// </summary>
[CreateAssetMenu(fileName = "ChangeStat", menuName = "Scriptable Objects/Gameplay/StatusEffect/ChangeStat")]
public class StatusEffect_ChangeStat : StatusEffectId
{
	[field: SerializeField] public StatId Stat { get; private set; }
	[field: SerializeField] public int Delta { get; private set; }

	public override void AppendTooltipText(StringBuilder sb)
	{
		sb.Append(TMPUtils.ColorizeLabelWithNumber($"{Stat.ShortName}: {{0}}", Delta));
	}
}
