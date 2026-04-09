using Character.Compositor;
using Reactivity;
using System;
using UnityEngine;

namespace Character.Creator.UI
{
	public class LightReferenceHandle : ReactiveBehaviour
	{
		private IColorActiveSelection _activeSelection;
		private ICustomizationSelectedDataRepository _dataRepo;
		private ColorAdjustmentSliderTarget _target;
		private ILightDarkSelection _lightDarkSelection;
		private RectTransform _rectTransform;

		private void Awake()
		{
			_activeSelection = this.GetComponentInParent<IColorActiveSelection>();
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_target = this.GetComponentInParent<IColorAdjustmentSlider>().Target;
			_lightDarkSelection = this.GetComponentInParent<ILightDarkSelection>();
			_rectTransform = this.GetComponent<RectTransform>();
		}

		private void Start()
		{
			AddReflector(Reflect);
		}

		private void Reflect()
		{
			var light = _lightDarkSelection.Light;
			this.gameObject.SetActive(!light);
			if (light) return;


			var id = _activeSelection.FirstSelected;
			if (!id) return;

			var val = GetTargetVal(_dataRepo.GetColorizeValues(id));
			_rectTransform.anchorMin = new(val, _rectTransform.anchorMin.y);
			_rectTransform.anchorMax = new(val, _rectTransform.anchorMax.y);

		}

		float GetTargetVal(IColorizeValues values)
		{
			var part = values.Base;
			if (_target == ColorAdjustmentSliderTarget.Hue) return part.Hue;
			else if (_target == ColorAdjustmentSliderTarget.Saturation) return part.Saturation;
			else if (_target == ColorAdjustmentSliderTarget.Value) return part.Value;
			else throw new ArgumentException("Invalid target");
		}
	}
}