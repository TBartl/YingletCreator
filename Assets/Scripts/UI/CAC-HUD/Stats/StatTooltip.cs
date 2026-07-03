using Reactivity;
using System.Text;

public class StatTooltip : Tooltip
{
	private IActiveCharacterProvider _activeCharacterProvider;
	private IStatReference _reference;
	private Computed<ICharacterStats> _stats;
	private Computed<string> _text;

	private void Start()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_reference = this.GetComponentInParentSafe<IStatReference>();
		_stats = CreateComputed<ICharacterStats>(ComputeStats);
		_text = CreateComputed<string>(ComputeText);
	}

	ICharacterStats ComputeStats()
	{
		var character = _activeCharacterProvider.ActiveExpeditionCharacter.Val;
		if (character == null) return null;
		return character.GetComponentInChildrenSafe<ICharacterStats>();
	}

	private string ComputeText()
	{
		var stats = _stats.Val;
		if (stats == null) return "";
		if (!IsCurrentlyShowing) return ""; // Little optimization to not string build if the tooltip isn't showing

		var sb = new StringBuilder();
		sb.AppendLine($"{_reference.Stat.DisplayName}");

		var statRecords = stats.GetStatRecords(_reference.Stat);
		foreach (var statRecord in statRecords)
		{
			var colorizedLabel = TMPUtils.ColorizeLabelWithNumber($"{statRecord.Source}: {{0}}", statRecord.Delta);
			sb.AppendLine($"<size=90%>{colorizedLabel}</size>");
		}
		//sb.AppendLine($"<size=90%>{stat.Value} {stat.Stat.LoadSync().ShortName}</size>");
		sb.Append(TMPUtils.FlavorStart);
		sb.AppendLine($"Stats are used in{TMPUtils.DiceSprite} dice rolls.");
		sb.Append($"At 0, you're incapacitated.");
		sb.AppendLine(TMPUtils.FlavorEnd);
		return sb.ToString();
	}

	public override string Text => _text.Val;
}
