using Character.Data;
using Reactivity;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class CharacterCreatorSlider : ReactiveBehaviour, IPointerUpHandler
	{
		[SerializeField] AssetReferenceT<CharacterSliderId> _sliderReference;
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorUndoManager _undoManager;
		private Slider _slider;
		private bool _recordDragValue = true;

		public CharacterSliderId SliderId => _sliderReference.LoadSync();

		private void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_slider = this.GetComponentInChildren<Slider>();
			_slider.onValueChanged.AddListener(Slider_OnValueChanged);
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
			_slider.SetValueWithoutNotify(_dataRepo.GetSliderValue(SliderId));
		}

		private void Slider_OnValueChanged(float arg0)
		{
			if (_recordDragValue)
			{
				// Only record to undo manager if we just started dragging this
				_undoManager.RecordState($"Adjust slider \"{SliderId.name}\"");
				_recordDragValue = false;
			}

			_dataRepo.SetSliderValue(SliderId, arg0);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_recordDragValue = true;
		}
	}
}