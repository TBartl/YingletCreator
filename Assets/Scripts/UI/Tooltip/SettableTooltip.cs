using Reactivity;
using UnityEngine;

// Not recommended to use this under conventional reactivity design
public class SettableTooltip : Tooltip
{
	[SerializeField, TextArea] string _initialText;
	Observable<string> _text = new Observable<string>();

	public override string Text => _text.Val;

	protected override void Awake()
	{
		base.Awake();
		_text.Val = _initialText;
		if (string.IsNullOrWhiteSpace(_initialText))
		{
			Debug.LogWarning($"Tooltip on GameObject '{gameObject.name}' has no text set.", this);
		}
	}

	public void SetText(string newText)
	{
		_text.Val = newText;
	}
}
