using UnityEngine;

namespace Character.Data
{
	/// <summary>
	/// Controls the preview portrait generated for characters
	/// </summary>
	[CreateAssetMenu(fileName = "Portrait", menuName = "Scriptable Objects/Character Data/PortraitId")]
	public class PortraitId : CharacterToggleComponent
	{
		[SerializeField] string _overrideName;
		public string DisplayName => string.IsNullOrWhiteSpace(_overrideName) ? name : _overrideName;

		[SerializeField] AnimationClip _pose;
		public AnimationClip Pose => _pose;

		[field: SerializeField]
		public PupilOffsets PupilOffsets { get; private set; }

		[SerializeField] CharacterTogglePreviewData _preview;
		public CharacterTogglePreviewData Preview => _preview;

		[SerializeField] int _orderIndex;
		public int OrderIndex => _orderIndex;
	}
}
