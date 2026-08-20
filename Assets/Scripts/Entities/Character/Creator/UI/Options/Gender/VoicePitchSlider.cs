using Reactivity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class VoicePitchSlider : ReactiveBehaviour, IPointerUpHandler, IExtendableSlider
	{
		[SerializeField] Vector2 _extendedRange;
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorUndoManager _undoManager;
		private Slider _slider;
		private bool _recordDragValue = true;
		private Vector2 _originalRange;

		private void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_slider = this.GetComponentInChildren<Slider>();
			_slider.onValueChanged.AddListener(Slider_OnValueChanged);
			_originalRange = new Vector2(_slider.minValue, _slider.maxValue);

		}

		private void Start()
		{
			AddReflector(ReflectSliderValue);
		}

		private new void OnDestroy()
		{
			base.OnDestroy();
			_slider.onValueChanged.RemoveListener(Slider_OnValueChanged);
		}

		private void ReflectSliderValue()
		{
			_slider.SetValueWithoutNotify(_dataRepo.CustomizationData.GenderData.VoicePitchShift.Val);
		}

		private void Slider_OnValueChanged(float arg0)
		{
			if (_recordDragValue)
			{
				// Only record to undo manager if we just started dragging this
				_undoManager.RecordState($"Change voice pitch");
				_recordDragValue = false;
			}

			_dataRepo.CustomizationData.GenderData.VoicePitchShift.Val = arg0;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_recordDragValue = true;
		}

		public void SetExtended(bool isExtended)
		{
			_slider.minValue = isExtended ? _extendedRange.x : _originalRange.x;
			_slider.maxValue = isExtended ? _extendedRange.y : _originalRange.y;
		}
	}
}
