using UnityEngine;

namespace Character.Data
{
	/// <summary>
	/// Controls the preview portrait generated for characters
	/// </summary>
	[CreateAssetMenu(fileName = "Stance", menuName = "Scriptable Objects/Character Data/StanceId")]
	public class StanceId : CharacterToggleComponent
	{
		[SerializeField] string _overrideName;
		public string DisplayName => string.IsNullOrWhiteSpace(_overrideName) ? name : _overrideName;

		[SerializeField] AnimationClip _idleAnim;
		public AnimationClip IdleAnim => _idleAnim;
	}
}
