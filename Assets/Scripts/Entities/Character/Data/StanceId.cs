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

		[SerializeField] AnimationClip _sleepAnim;
		public AnimationClip SleepAnim => _sleepAnim;

		[SerializeField] float _pupilOffsetY = 0f;
		public float PupilOffsetY => _pupilOffsetY;

		[SerializeField] float _heightOffset = 0f;
		public float HeightOffset => _heightOffset;

		[SerializeField] float _idleAnimSpeedMultiplier = 1;
		public float IdleAnimSpeedMultiplier => _idleAnimSpeedMultiplier;
	}
}
