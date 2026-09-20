using Encounters.Runtime;
using Reactivity;
using TMPro;
using UnityEngine;

public class ReflectRollUISumText : ReactiveBehaviour
{
	[SerializeField] private RollUISumType _sumType = RollUISumType.Expected;
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
		_text.text = GetSumString();
	}

	string GetSumString()
	{
		var sum = _sumType switch
		{
			RollUISumType.Expected => _data.ExpectedSum,
			RollUISumType.Real => _data.RealSum,
			_ => throw new System.NotImplementedException()
		};
		if (sum == 0) return "X";
		if (sum == RollProvider.MaxRollValue) return "<sprite name=\"Star\">";
		return sum.ToString();
	}
}