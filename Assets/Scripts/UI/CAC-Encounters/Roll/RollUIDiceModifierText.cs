using Encounters.Runtime;
using Reactivity;
using System.Linq;
using TMPro;

public class RollUIDiceModifierText : ReactiveBehaviour
{
	private RollNodeVisitData _data;
	private TMP_Text _text;

	void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;
		_text = this.GetComponentSafe<TMP_Text>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var state = _data.State;
		if (state == RollState.Prepare || state == RollState.Rolling) return;
		var diceRolls = _data.DiceRolls;
		if (diceRolls.All(x => x == 1))
		{
			_text.text = "X";
			return;
		}
		if (diceRolls.All(x => x == 6))
		{
			_text.text = "<sprite name=\"Star\">";
			return;
		}
		_text.text = "+" + _data.DiceRolls.Sum().ToString();
	}
}
