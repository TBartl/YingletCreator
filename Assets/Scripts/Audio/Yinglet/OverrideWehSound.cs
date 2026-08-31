using Character.Data;
using UnityEngine;

namespace Character.Compositor
{
	[CreateAssetMenu(fileName = "OverrideWehSound", menuName = "Scriptable Objects/Character Compositor/ToggleComponents/OverrideWehSound")]
	public class OverrideWehSound : CharacterToggleComponent
	{
		[SerializeField] private SoundEffectBase _sound;
		public SoundEffectBase Sound => _sound;
	}
}
