using Snapshotter;
using UnityEngine;

namespace Character.Data
{
	/// <summary>
	/// Controls the preview portrait generated for characters
	/// </summary>
	[CreateAssetMenu(fileName = "Portrait", menuName = "Scriptable Objects/Character Data/PortraitId")]
	public class PortraitId : ScriptableObject, IHasUniqueAssetId, ISnapshottableScriptableObject
	{
		[SerializeField, HideInInspector] string _uniqueAssetId;
		public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

		[SerializeField] string _overrideName;
		public string DisplayName => string.IsNullOrWhiteSpace(_overrideName) ? name : _overrideName;

		[SerializeField] AnimationClip _pose;
		public AnimationClip Pose => _pose;

		[SerializeField] CharacterTogglePreviewData _preview;
		public CharacterTogglePreviewData Preview => _preview;
	}
}
