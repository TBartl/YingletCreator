using Reactivity;
using TMPro;
using UnityEngine;


public class YingletHeightDisplay : ReactiveBehaviour
{
	private ISettingsManager _settingsManager;
	private IYingletHeightProvider _heightProvider;
	private TMP_Text _text;

	Observable<float> _rawHeight = new Observable<float>(0f);

	void Start()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
		_heightProvider = Singletons.GetSingleton<IYingletHeightProvider>();
		_text = this.GetComponent<TMP_Text>();
		AddReflector(ReflectText);
	}

	private void LateUpdate()
	{
		_rawHeight.Val = _heightProvider.YScale;
	}

	const float UnitsToMeters = 1.04f;
	const float KassenConversionFactor = 0.9626049f;

	void ReflectText()
	{
		float val = _rawHeight.Val;

		_text.text = _settingsManager.Settings.UnitSystem switch
		{
			UnitSystem.Metric => FormatMetric(val),
			UnitSystem.Imperial => FormatImperial(val),
			UnitSystem.Kassens => FormatKassens(val),
			_ => throw new System.Exception("Unsupported unit system")
		};
	}

	private static string FormatMetric(float units)
	{
		return $"{(units * UnitsToMeters).ToString("F2")}m";
	}

	private static string FormatKassens(float units)
	{
		return $"{(units / KassenConversionFactor).ToString("F2")} Kassens";
	}

	private static string FormatImperial(float units)
	{
		// Convert meters to total feet
		float totalFeet = units * UnitsToMeters * 3.28084f;
		int feet = Mathf.FloorToInt(totalFeet);
		float fractionalFeet = totalFeet - feet;

		// Convert remainder to inches and round to nearest whole inch
		int inches = Mathf.RoundToInt(fractionalFeet * 12f);

		// Handle rounding that produces 12 inches
		if (inches >= 12)
		{
			feet += 1;
			inches = 0;
		}

		return $"{feet}' {inches}\"";
	}
}
