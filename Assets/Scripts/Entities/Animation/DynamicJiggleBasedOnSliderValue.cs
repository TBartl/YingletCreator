using Character.Creator;
using Character.Data;
using JigglePhysics;
using Reactivity;
using UnityEngine;


public class DynamicJiggleBasedOnSliderValue : ReactiveBehaviour
{
	[SerializeField] JiggleSettings _baseJiggleSettings;
	[SerializeField] AssetReferenceT<CharacterSliderId> _sliderReference;
	[SerializeField] AnimationCurve _sliderValToBlendPercent;

	private ICustomizationDataRepository _dataRepo;
	private JiggleSettingsData _newSettingsData;
	private float _originalBlend;
	private JiggleSettings _newSettingsObject;

	private void Awake()
	{
		_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();

		// Create the settings data and cache the original blend
		_newSettingsData = _baseJiggleSettings.GetData();
		_originalBlend = _newSettingsData.blend;


		// Create a new jiggle settings that we inject into the jiggle rig builder
		// This should be early in the script execution order to ensure we're ahead of the JiggleRigBuilder's Awake
		var jiggleRigBuilder = this.GetComponent<JiggleRigBuilder>();

		_newSettingsObject = ScriptableObject.CreateInstance<JiggleSettings>();
		_newSettingsObject.name = _baseJiggleSettings.name + " (Dynamic)";

		foreach (var rig in jiggleRigBuilder.jiggleRigs)
		{
			if (rig.jiggleSettings == _baseJiggleSettings)
			{
				rig.jiggleSettings = _newSettingsObject;
			}
		}
	}

	private void Start()
	{
		AddReflector(ReflectSlider);
	}

	void ReflectSlider()
	{
		var sliderVal = _dataRepo.GetSliderValue(_sliderReference.LoadSync());
		_newSettingsData.blend = _sliderValToBlendPercent.Evaluate(sliderVal) * _originalBlend;
		_newSettingsObject.SetData(_newSettingsData);
	}
}
