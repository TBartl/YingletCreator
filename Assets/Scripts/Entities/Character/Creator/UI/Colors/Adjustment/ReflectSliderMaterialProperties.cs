using Character.Creator;
using Character.Creator.UI;
using Reactivity;
using UnityEngine;
using UnityEngine.UI;

public class ReflectSliderMaterialProperties : ReactiveBehaviour
{
	static readonly int HUE = Shader.PropertyToID("_Hue");
	static readonly int SATURATION = Shader.PropertyToID("_Saturation");
	static readonly int VALUE = Shader.PropertyToID("_Value");
	private ICustomizationSelectedDataRepository _dataRepo;
	private IColorActiveSelection _activeSelection;
	private ILightDarkSelection _lightDarkSelection;
	private Image _image;
	private Material _material;

	private void Awake()
	{
		_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		_activeSelection = this.GetComponentInParent<IColorActiveSelection>();
		_lightDarkSelection = this.GetComponentInParent<ILightDarkSelection>();
		_image = this.GetComponent<Image>();
		_material = new Material(_image.material);
		_image.material = _material;
	}

	private void Start()
	{
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var id = _activeSelection.FirstSelected;
		if (id == null) return;

		var colors = _dataRepo.GetColorizeValues(id);
		var color = _lightDarkSelection.Light ? colors.Base : colors.Shade;

		_material.SetFloat(HUE, color.Hue);
		_material.SetFloat(SATURATION, color.Saturation);
		_material.SetFloat(VALUE, color.Value);
		_image.SuperDirtyMaterial();
	}
}
