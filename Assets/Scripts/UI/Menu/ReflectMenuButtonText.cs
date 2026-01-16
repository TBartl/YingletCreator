using Reactivity;
using System;
using TMPro;

public class ReflectMenuButtonText : ReactiveBehaviour
{
	private IMenuButtonTextSelection _selection;
	private TMP_Text _text;

	void Start()
	{
		_selection = this.GetComponent<IMenuButtonTextSelection>();
		_text = this.GetComponent<TMP_Text>();
		AddReflector(ReflectText);
	}

	private void ReflectText()
	{
		// Keep text the same if this has turned null so we can fade out nicely
		if (_selection.HoveredMenuButton == null) return;

		_text.text = _selection.HoveredMenuButton?.Type switch
		{
			MenuButtonType.Exit => "Exit",
			MenuButtonType.Settings => "Settings",
			MenuButtonType.About => "About",
			MenuButtonType.Discord => "Discord",
			MenuButtonType.Screensaver => "Screensaver",
			_ => throw new ArgumentException($"Unknown MenuButtonType: {_selection.HoveredMenuButton?.Type}")
		};
	}
}
