using Reactivity;
using System.Text;

public class StatTooltip : Tooltip
{
	private IStatReference _reference;
	private Computed<string> _text;

	private void Start()
	{
		_reference = this.GetComponentInParentSafe<IStatReference>();
		_text = CreateComputed<string>(ComputeText);
	}

	private string ComputeText()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{_reference.Stat.DisplayName}");
		//sb.AppendLine($"<size=90%>{stat.Value} {stat.Stat.LoadSync().ShortName}</size>");
		sb.Append(TMPUtils.FlavorStart);
		sb.AppendLine($"Stats are used in{TMPUtils.DiceSprite} dice rolls.");
		sb.Append($"At 0, you're incapacitated.");
		sb.AppendLine(TMPUtils.FlavorEnd);
		return sb.ToString();
	}

	public override string Text => _text.Val;
}
