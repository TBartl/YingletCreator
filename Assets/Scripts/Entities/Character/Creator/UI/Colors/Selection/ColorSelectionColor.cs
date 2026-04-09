using Character.Compositor;
using Reactivity;
using UnityEngine;
using UnityEngine.UI;


namespace Character.Creator.UI
{
	public class ColorSelectionColor : ReactiveBehaviour
	{
		private ICustomizationSelectedDataRepository _dataRepository;
		private IColorSelectionReference _reference;
		private Image _image;
		private Material _material;

		static readonly int BASE_COLOR_PROPERTY_ID = Shader.PropertyToID("_Base");
		static readonly int SHADE_COLOR_PROPERTY_ID = Shader.PropertyToID("_Shade");

		private void Awake()
		{
			_dataRepository = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_reference = this.GetComponentInParent<IColorSelectionReference>();
			_image = this.GetComponent<Image>();
		}
		private void Start()
		{
			_material = new Material(_image.materialForRendering);
			_image.material = _material;
			_image.color = Color.white;
			AddReflector(Reflect);
		}

		private void Reflect()
		{
			var colorizeValues = _dataRepository.GetColorizeValues(_reference.Id);

			_material.SetColor(BASE_COLOR_PROPERTY_ID, colorizeValues.Base.GetColor());
			_material.SetColor(SHADE_COLOR_PROPERTY_ID, colorizeValues.Shade.GetColor());
			_image.SuperDirtyMaterial();
		}
	}
}