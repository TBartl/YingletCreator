using Character.Creator;
using Character.Creator.UI;
using Character.Data;
using Reactivity;
using System.Linq;
using UnityEngine;

public class DisableSliderOnSelectToggles : ReactiveBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;

	private ICustomizationSelectedDataRepository _dataRepository;
	private CharacterCreatorSlider _characterCreatorSlider;
	private CanvasGroup _canvasGroup;
	Computed<bool> _enabled;
	private Coroutine _transitionCoroutine;

	private void Start()
	{
		_dataRepository = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		_characterCreatorSlider = this.GetComponent<CharacterCreatorSlider>();
		_canvasGroup = this.GetComponent<CanvasGroup>();
		_enabled = CreateComputed(ComputeEnabled);
		AddReflector(Reflect);
	}

	private bool ComputeEnabled()
	{
		var toggles = _dataRepository.CustomizationData.ToggleData.Toggles.ToArray();
		foreach (var toggle in toggles)
		{
			foreach (var component in toggle.Components)
			{
				if (component is ToggleDisablesSlider disablesSlider && disablesSlider.SliderToDisable == _characterCreatorSlider.SliderId)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void Reflect()
	{
		bool enabled = _enabled.Val;
		_canvasGroup.interactable = enabled;
		float from = _canvasGroup.alpha;
		float to = enabled ? 1 : 0f;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _canvasGroup.alpha = Mathf.Lerp(from, to, p));
	}
}
