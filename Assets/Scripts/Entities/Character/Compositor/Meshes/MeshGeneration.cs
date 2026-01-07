using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Compositor
{
	public interface IMeshGeneration
	{
		IEnumerable<MeshObjectWithMaterialDescription> Meshes { get; }

	}
	public class MeshGeneration : ReactiveBehaviour, IMeshGeneration
	{
		[SerializeField] Transform _rigRoot;
		private IMeshGatherer _meshDefinition;
		private EnumerableDictReflector<MeshWithMaterial, MeshObjectWithMaterialDescription> _enumerableReflector;
		private Dictionary<string, Transform> _boneMap;

		const float ExtentsFactor = 1;

		IEnumerable<MeshObjectWithMaterialDescription> IMeshGeneration.Meshes => _enumerableReflector.Values;

		void Awake()
		{
			_meshDefinition = this.GetComponent<IMeshGatherer>();
			_enumerableReflector = new EnumerableDictReflector<MeshWithMaterial, MeshObjectWithMaterialDescription>(Added, Removed);
			_boneMap = GetBoneMap(_rigRoot);
			AddReflector(Composite);
		}
		private MeshObjectWithMaterialDescription Added(MeshWithMaterial mesh)
		{
			var newGO = GameObject.Instantiate(mesh.SkinnedMeshRendererPrefab, this.transform.position, Quaternion.identity, this.transform);

			newGO.name = mesh.SkinnedMeshRendererPrefab.name;

			// GameObject.Instantiate sets the global transform, but we want to ensure the mesh has an identity local transform.
			newGO.transform.localPosition = Vector3.zero;
			newGO.transform.localRotation = Quaternion.identity;
			newGO.transform.localScale = Vector3.one;

			var skinnedMeshRenderer = newGO.GetComponent<SkinnedMeshRenderer>();

			// Despite the name, this doesn't actually have to be a skinned mesh renderer. Particularly, props aren't weighted
			if (skinnedMeshRenderer != null)
			{
				skinnedMeshRenderer.rootBone = _boneMap["root"];
				skinnedMeshRenderer.bones = skinnedMeshRenderer.bones.Select(b =>
				{
					return _boneMap[b.name];
				}
				).ToArray();

				// We have dynamic animations, so we need to do extend the bounds a lil' bit
				var newBounds = skinnedMeshRenderer.localBounds;
				newBounds.extents += Vector3.one * ExtentsFactor;
				skinnedMeshRenderer.localBounds = newBounds;
			}
			if (mesh.BoneToAttachTo != null)
			{
				if (_boneMap.TryGetValue(mesh.BoneToAttachTo.BoneName, out var boneTransform))
				{
					newGO.transform.SetParent(boneTransform, false);
					newGO.transform.localPosition = Vector3.zero;
					newGO.transform.localRotation = Quaternion.identity;
					newGO.transform.localScale = Vector3.one;
				}
				else
				{
					Debug.LogWarning($"Bone {mesh.BoneToAttachTo} not found in rig.");
				}
			}

			return new MeshObjectWithMaterialDescription(newGO, mesh.MaterialDescription);
		}
		private void Removed(MeshObjectWithMaterialDescription obj)
		{
			Destroy(obj.MeshGO);
		}

		public void Composite()
		{
			var meshes = _meshDefinition.AllRelevantMeshes.ToArray();
			_enumerableReflector.Enumerate(meshes);
		}


		public static Dictionary<string, Transform> GetBoneMap(Transform root)
		{
			var transformMap = new Dictionary<string, Transform>();
			transformMap[root.name] = root;
			AddChildrenToMap(root);
			return transformMap;

			void AddChildrenToMap(Transform parent)
			{
				foreach (Transform child in parent)
				{
					transformMap[child.name] = child;
					AddChildrenToMap(child);
				}
			}
		}
	}

	public sealed class MeshObjectWithMaterialDescription
	{
		public MeshObjectWithMaterialDescription(GameObject mesh, MaterialDescription materialDescription)
		{
			MeshGO = mesh;
			MaterialDescription = materialDescription;
		}

		public GameObject MeshGO { get; }
		public MaterialDescription MaterialDescription { get; }
	}
}
