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
		if (Delta >= 0)
		{
			sb.Append('+');
		}
		sb.Append($"{Delta} {Stat.DisplayName}");
	}
}
