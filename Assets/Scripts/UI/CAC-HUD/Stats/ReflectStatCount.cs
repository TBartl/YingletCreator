using Reactivity;
using System.Linq;
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

		var stat = _reference.Stat;
		int statValue = _stats.Val.GetStat(stat);
		var statRecords = _stats.Val.GetStatRecords(stat);
		int defaultValue = statRecords.First().Delta; // Kind of a hacky way to assume the baseline value, but the class should always be first
		_text.text = TMPUtils.ColorizeNumber(statValue, defaultValue);
	}
}
