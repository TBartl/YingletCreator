using UnityEngine;


namespace Character.Data
{
	// Used for certain hairstyles that don't really support hair length
	[CreateAssetMenu(fileName = "ToggleDisablesSlider", menuName = "Scriptable Objects/Character Data/ToggleComponents/ToggleDisablesSlider")]
	public class ToggleDisablesSlider : CharacterToggleComponent
	{
		[SerializeField] AssetReferenceT<CharacterSliderId> _sliderReference;
		public CharacterSliderId SliderToDisable => _sliderReference.LoadSync();
	}
}