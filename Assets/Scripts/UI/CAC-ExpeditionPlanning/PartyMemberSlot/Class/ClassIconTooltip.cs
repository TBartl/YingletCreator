using Reactivity;
using System.Text;

public class ClassIconTooltip : Tooltip
{
	private IClassReference _reference;
	private Computed<string> _text;

	private void Start()
	{
		_reference = this.GetComponentInParentSafe<IClassReference>();
		_text = CreateComputed<string>(ComputeText);
	}

	private string ComputeText()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Class: {_reference.Class.name}");
		var stats = _reference.Class.Stats;
		foreach (var stat in stats)
		{
			sb.AppendLine($"<size=90%>{stat.Value} {stat.Stat.LoadSync().ShortName}</size>");
		}
		sb.AppendLine($"<color=#FFFFFF99><size=70%>Stats are used in\n{TMPUtils.DiceSprite} dice rolls.</size></color>");
		return sb.ToString();
	}

	public override string Text => _text.Val;
}
