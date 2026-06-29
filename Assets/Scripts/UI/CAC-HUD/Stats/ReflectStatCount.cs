using Reactivity;
using TMPro;

public class ReflectStatCount : ReactiveBehaviour
{
	private IActiveCharacterProvider _activeCharacterProvider;
	private IStatReference _reference;
	private TMP_Text _text;
	private Computed<ICharacterStats> _stats;

	private void Start()
	{
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();

		_reference = this.GetComponentInParentSafe<IStatReference>();
		_text = this.GetComponentSafe<TMPro.TMP_Text>();

		_stats = CreateComputed<ICharacterStats>(ComputeStats);
		AddReflector(Reflect);
	}

	ICharacterStats ComputeStats()
	{
		var character = _activeCharacterProvider.ActiveExpeditionCharacter.Val;
		if (character == null) return null;
		return character.GetComponentInChildrenSafe<ICharacterStats>();
	}

	private void Reflect()
	{
		if (_stats.Val == null)
		{
			return;
		}

		_text.text = _stats.Val.GetStat(_reference.Stat).ToString();
	}
}
