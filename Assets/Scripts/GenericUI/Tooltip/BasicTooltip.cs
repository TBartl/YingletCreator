using UnityEngine;


public class BasicTooltip : Tooltip
{
	[SerializeField, TextArea] string _text;

	public override string Text => _text;

	protected override void Awake()
	{
		base.Awake();
		if (string.IsNullOrWhiteSpace(_text))
		{
			Debug.LogWarning($"Tooltip on GameObject '{gameObject.name}' has no text set.", this);
		}
	}
}
