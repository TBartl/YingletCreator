using Reactivity;
using TMPro;

public class ReflectRoundNumber : ReactiveBehaviour
{
	private IGlobalRoundProvider _roundProvider;
	private TMP_Text _text;

	void Start()
	{
		_roundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		_text = GetComponent<TMP_Text>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		if (_roundProvider.RoundManager == null)
		{
			return;
		}
		_text.text = _roundProvider.RoundManager.CurrentRound.Val.ToString();
	}
}
