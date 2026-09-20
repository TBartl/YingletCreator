using Encounters.Runtime;
using Reactivity;
using UnityEngine;
using UnityEngine.UI;

public enum RollUISumType
{
	Expected,
	Real
}

public enum RollUISumColor
{
	Light,
	Dark
}

public class ReflectRollUISumColor : ReactiveBehaviour
{
	[SerializeField] private RollUISettings _settings;
	[SerializeField] private RollUISumType _sumType = RollUISumType.Expected;
	[SerializeField] private RollUISumColor _colorType = RollUISumColor.Light;

	private Graphic _graphic;
	private RollNode _node;
	private RollNodeVisitData _data;

	void Start()
	{
		_graphic = this.GetComponentSafe<Graphic>();
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_node = reference.Record.Node as RollNode;
		_data = reference.Record.VisitData as RollNodeVisitData;

		AddReflector(Reflect);
	}
	void Reflect()
	{
		var sum = _sumType == RollUISumType.Expected ? _data.ExpectedSum : _data.RealSum;
		var branch = _node.Branches.GetBranch(sum);

		var colorSettings = _settings.RollClassificationColorMap[branch.Classification];
		var color = _colorType == RollUISumColor.Light ? colorSettings.BackgroundColor : colorSettings.TextColor;

		_graphic.color = color;
	}
}
