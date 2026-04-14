using Character.Compositor;
using Snapshotter;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Character.Data
{
	[CreateAssetMenu(fileName = "Pose", menuName = "Scriptable Objects/Character Data/PoseId")]
	public class PoseId : ScriptableObject, IHasUniqueAssetId, ISnapshottableScriptableObject, IGroupedOrderableScriptableObject<PoseOrderGroup>
	{
		[SerializeField, HideInInspector] string _uniqueAssetId;
		public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

		[SerializeField] AnimationClip _clip;
		public AnimationClip Clip => _clip;

		// Filenames can't contain special characters I want for some things
		[SerializeField] string _overrideName;
		public string OverrideName => _overrideName;

		[SerializeField] AssetReferenceT<MeshWithMaterial>[] _propReferences;
		public IEnumerable<MeshWithMaterial> Props => _propReferences.Select(r => r.LoadSync());

		[SerializeField] CharacterTogglePreviewData _preview;
		public CharacterTogglePreviewData Preview => _preview;

		public string DisplayName => string.IsNullOrWhiteSpace(_overrideName) ? name : _overrideName;


		[SerializeField] PoseOrderData _order;
		public PoseOrderData Order => _order;
		IGroupedOrderData<PoseOrderGroup> IGroupedOrderableScriptableObject<PoseOrderGroup>.Order => Order;
	}

	[System.Serializable]
	public class PoseOrderData : IGroupedOrderData<PoseOrderGroup>
	{
		[SerializeField] AssetReferenceT<PoseOrderGroup> _groupReference;
		public PoseOrderGroup Group => _groupReference.LoadSync();

		[SerializeField] int _index;
		public int Index => _index;
	}
}