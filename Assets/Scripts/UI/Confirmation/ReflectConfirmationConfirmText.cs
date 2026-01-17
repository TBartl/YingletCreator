using Reactivity;
using TMPro;

public class ReflectConfirmationConfirmText : ReactiveBehaviour
{
	private IConfirmationManager _confirmationManager;
	private TMP_Text _text;

	void Start()
	{
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
		_text = this.GetComponent<TMP_Text>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var current = _confirmationManager.Current.Val;
		if (current == null) return; // We can just leave the text as-is

		_text.text = current.ConfirmText;
	}
}
