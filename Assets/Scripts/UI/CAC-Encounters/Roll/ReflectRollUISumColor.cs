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
	Dark,
	Juicy,
	HalfJuicy
}

public class ReflectRollUISumColor : ReactiveBehaviour
{
	[SerializeField] private RollUISettings _settings;
	[SerializeField] private RollUISumType _sumType = RollUISumType.Expected;
	[SerializeField] private RollUISumColor _colorType = RollUISumColor.Light;

	private Graphic _graphic;
	private RollNodeVisitData _data;

	void Start()
	{
		_graphic = this.GetComponentSafe<Graphic>();
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;

		AddReflector(Reflect);
	}
	void Reflect()
	{
		var branch = _sumType == RollUISumType.Expected ? _data.ExpectedBranch : _data.RealBranch;

		var colorSettings = _settings.RollClassificationColorMap[branch.Classification];
		var color = _colorType switch
		{
			RollUISumColor.Light => colorSettings.BackgroundColor,
			RollUISumColor.Dark => colorSettings.TextColor,
			RollUISumColor.Juicy => colorSettings.JuicyColor,
			RollUISumColor.HalfJuicy => new Color(colorSettings.JuicyColor.r, colorSettings.JuicyColor.g, colorSettings.JuicyColor.b, 0.5f),
			_ => throw new System.NotImplementedException($"Color type {_colorType} not implemented")
		};

		_graphic.color = color;
	}
}
