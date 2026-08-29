
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
		var image = this.GetComponentsInChildrenSafe<Image>();
		image[1].sprite = status.Icon; // Hackily skip past this own image

		var sb = new StringBuilder();
		sb.Append(status.DisplayName);

		bool firstEffect = true;

		foreach (var effect in status.StatusEffects)
		{
			string text = effect.GetTooltipText();
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			if (firstEffect)
			{
				sb.Append("<line-height=130%>");
				sb.AppendLine();
				sb.Append("</line-height>");
				firstEffect = false;
			}
			else
			{
				sb.AppendLine();
			}
			sb.Append(text);
		}
		_text = sb.ToString();
	}
}
