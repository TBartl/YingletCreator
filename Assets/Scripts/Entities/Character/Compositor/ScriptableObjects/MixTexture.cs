using Character.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Character.Compositor
{

	public interface IMixTexture : ITaggableCharacterElement
	{

		/// <summary>
		/// The ID used to re-color this
		/// If null, this mix texture can't be recolored
		/// </summary>
		ReColorId ReColorId { get; }

		/// <summary>
		/// What material this texture should be applied to
		/// </summary>
		MaterialDescription TargetMaterialDescription { get; }

		/// <summary>
		/// Texture that contains the shading for this. Usually a reddish gradient 
		/// </summary>
		Texture2D Grayscale { get; }

		/// <summary>
		/// Optional texture that controls what parts of the texture should be rendered
		/// </summary>
		Texture2D Mask { get; }

		string name { get; }

		bool Sortable { get; }

		TargetMaterialTexture TargetMaterialTexture { get; }
	}

	[CreateAssetMenu(fileName = "MixTexture", menuName = "Scriptable Objects/Character Compositor/MixTexture")]
	public class MixTexture : ScriptableObject, IMixTexture, IGroupedOrderableScriptableObject<MixTextureOrderGroup>
	{
		[SerializeField] AssetReferenceT<ReColorId> _reColorIdReference;
		[SerializeField] AssetReferenceT<MaterialDescription> _targetMaterialDescriptionReference;
		[SerializeField] AssetReferenceT<Texture2D> _grayscaleReference;
		[SerializeField] AssetReferenceT<Texture2D> _maskReference;
		[SerializeField] AssetReferenceT<CharacterElementTag>[] _tagReferences;
		[SerializeField] MixTextureOrderData _order;

		public ReColorId ReColorId => _reColorIdReference.LoadSync();

		public MaterialDescription TargetMaterialDescription => _targetMaterialDescriptionReference.LoadSync();

		public Texture2D Grayscale => _grayscaleReference.LoadSync();

		public Texture2D Mask => _maskReference.LoadSync();

		public bool Sortable => true;

		public virtual TargetMaterialTexture TargetMaterialTexture => TargetMaterialTexture.MainTexture;

		public IEnumerable<CharacterElementTag> Tags => _tagReferences.Select(r => r.LoadSync());

		public MixTextureOrderData Order => _order;
		IGroupedOrderData<MixTextureOrderGroup> IGroupedOrderableScriptableObject<MixTextureOrderGroup>.Order => Order;
	}

	public enum TargetMaterialTexture
	{
		MainTexture,
		Eyelid, // Used in the eye material; covers the pupil
		Pupil, // Used in the eye material
		Mouth, // Used in the mouth material; includes line and teeth
		MouthMask, // Used in the mouth material; the center that designates what should be alpha clipped out
	}

	[System.Serializable]
	public class MixTextureOrderData : IGroupedOrderData<MixTextureOrderGroup>
	{
		[SerializeField] AssetReferenceT<MixTextureOrderGroup> _groupReference;
		public MixTextureOrderGroup Group => _groupReference.LoadSync();

		[SerializeField] int _index;
		public int Index => _index;
	}
}
