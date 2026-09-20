using Encounters.Runtime;
using Reactivity;
using TMPro;

public class ReflectRollUIRealSum : ReactiveBehaviour
{
	private RollNodeVisitData _data;
	private TMP_Text _text;

	void Start()
	{
		_text = this.GetComponentSafe<TMP_Text>();
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		_text.text = _data.RealSum.ToString();
	}
}
