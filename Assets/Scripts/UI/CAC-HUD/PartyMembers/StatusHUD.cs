
using System.Text;
using UnityEngine.UI;

public interface IStatusHUD
{
	void SetStatus(StatusId status);
}

public class StatusHUD : Tooltip, IStatusHUD
{
	string _text = "";

	public override string Text => _text;

	public void SetStatus(StatusId status)
	{
		var image = this.GetComponentSafe<Image>();
		image.sprite = status.Icon;

		var sb = new StringBuilder();
		sb.Append(status.DisplayName);
		foreach (var effect in status.StatusEffects)
		{
			sb.AppendLine();
			effect.AppendTooltipText(sb);
		}
		_text = sb.ToString();
	}
}
