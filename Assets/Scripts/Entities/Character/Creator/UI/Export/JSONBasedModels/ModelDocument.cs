using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum MeshObjectOptimization
{
	OneMeshTwoSurfaces,
	OneMeshManySurfaces,
	TwoMeshes,
	ManyMeshes,
}

public enum ModelBaseFormat
{
	G3MF,
	GLTF
}

public class ModelDocument
{
	/// <summary>
	/// The list of accessors in this model.
	/// </summary>
	public List<ModelAccessor> accessors = new List<ModelAccessor>();
	/// <summary>
	/// The list of animations in this model. Yinglet Creator only uses this for eye texture transform animations.
	/// </summary>
	public List<ModelAnimation> animations = new List<ModelAnimation>();
	/// <summary>
	/// The binary buffer data for this model. Yinglet Creator only uses a single buffer for simplicity.
	/// </summary>
	public List<byte> bufferData = new List<byte>();
	/// <summary>
	/// The list of buffer views in this model.
	/// </summary>
	public List<ModelBufferView> bufferViews = new List<ModelBufferView>();
	/// <summary>
	/// The list of materials in this model.
	/// </summary>
	public List<ModelMaterial> materials = new List<ModelMaterial>();
	/// <summary>
	/// The list of meshes in this model.
	/// </summary>
	public List<ModelMesh> meshes = new List<ModelMesh>();
	/// <summary>
	/// The list of nodes in this model. The first node is the root node.
	/// </summary>
	public List<ModelNode> nodes = new List<ModelNode>();
	/// <summary>
	/// The list of spring rigs in this model.
	/// </summary>
	public List<ModelSpringRig> springRigs = new List<ModelSpringRig>();
	/// <summary>
	/// The list of textures in this model.
	/// </summary>
	public List<ModelTexture> textures = new List<ModelTexture>();
	/// <summary>
	/// A map from Transforms to their corresponding node indices in the nodes list.
	/// </summary>
	public Dictionary<Transform, int> transformNodeIndexMap = new Dictionary<Transform, int>();
	/// <summary>
	/// Unique names to ensure no duplicate names exist.
	/// </summary>
	private HashSet<string> uniqueNames = new HashSet<string>();
	private Dictionary<int, int> nodeIndexToJointMap = null;
	private int thumbnailTextureAndImageIndex = -1;

	/// <summary>
	/// If true, inject extra LowerLeg nodes between UpperLeg and LowerLeg1 to improve compatibility
	/// with applications that do not properly handle digitigrade legs on bipedal characters.
	/// </summary>
	private const bool INJECT_EXTRA_LOWER_LEG = true;

	/// <summary>
	/// Creates a ModelDocument from the given Yinglet.
	/// The returned data will be in a right-handed coordinate system.
	/// </summary>
	public static ModelDocument CreateFromYingletSkeleton(Transform skeletonHips)
	{
		ModelDocument doc = new ModelDocument();
		// Add the skeleton nodes.
		ModelNode root = new ModelNode();
		doc.nodes.Add(root);
		root.children.Add(1); // Will be the index of Hips after the next line.
		doc.ConvertUnityTransformsToNodesRecursive(skeletonHips, root);
		// Add all skeleton joints to the root node's skeletonJoints list.
		root.skeletonJoints = new List<int>();
		for (int i = 1; i < doc.nodes.Count; i++)
		{
			root.skeletonJoints.Add(i);
		}
		return doc;
	}

	public void ConvertYingletMeshes(Transform yingletRoot, MeshObjectOptimization meshOptimization, EyeExpression eyeExpression)
	{
		ModelNode root = nodes[0];
		// Add the meshes.
		SkinnedMeshRenderer[] skinnedMeshes = yingletRoot.GetComponentsInChildren<SkinnedMeshRenderer>();
		Dictionary<Material, int> uniqueMaterialToIndexMap = new Dictionary<Material, int>();
		Rect leftEyeRect = new Rect(0, 0, 1, 1);
		Rect rightEyeRect = new Rect(0, 0, 1, 1);
		Rect pupilRect = new Rect(0, 0, 1, 1);
		if (meshOptimization == MeshObjectOptimization.ManyMeshes)
		{
			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				SkinnedMeshRenderer skinnedMesh = skinnedMeshes[i];
				if (skinnedMesh.gameObject.name == "Eye-Left" || skinnedMesh.gameObject.name == "Eye-Right")
				{
					continue; // Eyes are handled later.
				}
				ModelNode meshNode = new ModelNode();
				meshNode.name = ReserveUniqueName(skinnedMesh.gameObject.name);
				meshNode.parent = 0;
				root.children.Add(nodes.Count); // Will be the index of the skinned mesh node after the next line.
				nodes.Add(meshNode);
				SkinnedMeshRenderer[] single = new SkinnedMeshRenderer[1] { skinnedMesh };
				ModelMesh meshResource = ModelMesh.FromNonEyeSkinnedMeshRenderers(this, single, true);
				meshNode.mesh = meshes.Count; // Will be the index of the skinned mesh node after the next line.
				meshResource.name = ReserveUniqueName(skinnedMesh.gameObject.name + "MeshData");
				meshes.Add(meshResource);
				if (uniqueMaterialToIndexMap.ContainsKey(skinnedMesh.sharedMaterial))
				{
					meshResource.surfaces[0].materialIndex = uniqueMaterialToIndexMap[skinnedMesh.sharedMaterial];
				}
				else
				{
					int matIndex = ModelMaterial.FromSingleMaterial(this, skinnedMesh.sharedMaterial);
					uniqueMaterialToIndexMap[skinnedMesh.sharedMaterial] = matIndex;
					meshResource.surfaces[0].materialIndex = matIndex;
				}
			}
		}
		else // One main mesh.
		{
			ModelNode meshNode = new ModelNode();
			meshNode.name = ReserveUniqueName("Mesh");
			meshNode.parent = 0;
			root.children.Add(nodes.Count); // Will be the index of the skinned mesh node after the next line.
			nodes.Add(meshNode);
			bool mergeSurfaces = meshOptimization != MeshObjectOptimization.OneMeshManySurfaces; // Not many surfaces.
			ModelMesh meshResource = ModelMesh.FromNonEyeSkinnedMeshRenderers(this, skinnedMeshes, mergeSurfaces);
			meshNode.mesh = meshes.Count; // Will be the index of the skinned mesh node after the next line.
			meshResource.name = ReserveUniqueName("YingletMeshData");
			meshes.Add(meshResource);
			if (mergeSurfaces)
			{
				ModelMaterial materialResource = ModelMaterial.AtlasMultipleMaterials(skinnedMeshes, meshResource.surfaces[0], ref leftEyeRect, ref rightEyeRect, ref pupilRect);
				materialResource.name = ReserveUniqueName("YingletMaterial");
				meshResource.surfaces[0].materialIndex = materials.Count;
				materials.Add(materialResource);
			}
			else // Many surfaces.
			{
				int surfaceIndex = 0;
				for (int i = 0; i < skinnedMeshes.Length; i++)
				{
					SkinnedMeshRenderer skinnedMesh = skinnedMeshes[i];
					if (skinnedMesh.gameObject.name == "Eye-Left" || skinnedMesh.gameObject.name == "Eye-Right")
					{
						continue; // Eyes are handled later.
					}
					// Deduplicate identical materials.
					if (uniqueMaterialToIndexMap.ContainsKey(skinnedMesh.sharedMaterial))
					{
						meshResource.surfaces[surfaceIndex].materialIndex = uniqueMaterialToIndexMap[skinnedMesh.sharedMaterial];
					}
					else
					{
						int matIndex = ModelMaterial.FromSingleMaterial(this, skinnedMesh.sharedMaterial);
						uniqueMaterialToIndexMap[skinnedMesh.sharedMaterial] = matIndex;
						meshResource.surfaces[surfaceIndex].materialIndex = matIndex;
					}
					surfaceIndex++;
				}
			}
		}
		// Special case: Add the eye bases separately so their texture maps can be animated properly.
		{
			SkinnedMeshRenderer[] eyeMeshes = new SkinnedMeshRenderer[2];
			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				if (skinnedMeshes[i].gameObject.name == "Eye-Left")
				{
					eyeMeshes[0] = skinnedMeshes[i];
				}
				else if (skinnedMeshes[i].gameObject.name == "Eye-Right")
				{
					eyeMeshes[1] = skinnedMeshes[i];
				}
			}
			ModelMesh.ModelMeshSurface eyesSurface = ModelMesh.SurfaceFromEyeMeshes(this, eyeMeshes);
			// Get the material for the eyes, either a new atlas of the two eyes, or a duplicate of the main material.
			if (meshOptimization == MeshObjectOptimization.OneMeshTwoSurfaces || meshOptimization == MeshObjectOptimization.TwoMeshes)
			{
				// Use a material with the same texture as the main mesh, but duplicated to allow independent UV animation.
				eyesSurface.materialIndex = materials[0].DuplicateAndAddModelMaterial(this, "YingletEyesMaterial");
			}
			else // MeshObjectOptimization.OneMeshManySurfaces or MeshObjectOptimization.ManyMeshes
			{
				// Use a new material created by atlasing the two eye materials.
				ModelMaterial mat = ModelMaterial.AtlasMultipleMaterials(eyeMeshes, eyesSurface, ref leftEyeRect, ref rightEyeRect, ref pupilRect);
				mat.name = ReserveUniqueName("YingletEyesMaterial");
				eyesSurface.materialIndex = materials.Count;
				materials.Add(mat);
			}
			// Animate the eye textures using KHR_texture_transform.
			materials[eyesSurface.materialIndex].isTargetOfTextureAnimation = true;
			ModelAnimation resetAnim = ModelAnimation.FromEyeExpression(this, eyesSurface.materialIndex, leftEyeRect, eyeExpression, eyeExpression);
			resetAnim.name = ReserveUniqueName("RESET");
			animations.Add(resetAnim);
			string[] eyeExpressionNames = new string[]
			{
				"Neutral",
				"Squint",
				"Blink",
				"Pleased",
				"Shocked",
				"Angry",
				"Sad",
				"Wince"
			};
			for (int expression = 0; expression < 8; expression++)
			{
				EyeExpression thisEyeExpression = (EyeExpression)expression;
				ModelAnimation animation = ModelAnimation.FromEyeExpression(this, eyesSurface.materialIndex, leftEyeRect, thisEyeExpression, eyeExpression);
				animation.name = ReserveUniqueName("Eyes" + eyeExpressionNames[expression]);
				animations.Add(animation);
			}
			// Make the UVs use the same rects as in the atlas, if any.
			leftEyeRect = AdjustRectForEyeExpression(leftEyeRect, eyeExpression);
			rightEyeRect = AdjustRectForEyeExpression(rightEyeRect, eyeExpression);
			pupilRect = AdjustRectForEyeExpression(pupilRect, eyeExpression);
			List<Vector2> eyesTextureMap = new List<Vector2>();
			ModelMaterial.RefitTextureMapIntoRect(eyesTextureMap, eyeMeshes[0], leftEyeRect);
			ModelMaterial.RefitTextureMapIntoRect(eyesTextureMap, eyeMeshes[1], rightEyeRect);
			ModelMaterial.RefitTextureMapIntoRect(eyesTextureMap, eyeMeshes[0], pupilRect);
			ModelMaterial.RefitTextureMapIntoRect(eyesTextureMap, eyeMeshes[1], pupilRect);
			eyesSurface.textureMap = eyesTextureMap;
			// Add the base eye mesh as either a new surface or a new mesh, depending on optimization setting.
			switch (meshOptimization)
			{
				case MeshObjectOptimization.OneMeshTwoSurfaces:
				case MeshObjectOptimization.OneMeshManySurfaces:
					meshes[0].surfaces.Add(eyesSurface);
					break;
				case MeshObjectOptimization.TwoMeshes:
				case MeshObjectOptimization.ManyMeshes:
					ModelNode eyesMeshNode = new ModelNode();
					eyesMeshNode.name = ReserveUniqueName("YingletEyesMesh");
					eyesMeshNode.parent = 0;
					root.children.Add(nodes.Count); // Will be the index of the skinned mesh node after the next line.
					nodes.Add(eyesMeshNode);
					ModelMesh eyesMesh = new ModelMesh();
					eyesMesh.surfaces.Add(eyesSurface);
					eyesMesh.name = ReserveUniqueName("YingletEyesMeshData");
					eyesMeshNode.mesh = meshes.Count; // Will be the index of the mesh resource after the next line.
					meshes.Add(eyesMesh);
					break;
			}
		}
		// Optional optimization pass, deduplicate identical vertices within each mesh surface.
		for (int meshIndex = 0; meshIndex < meshes.Count; meshIndex++)
		{
			ModelMesh mesh = meshes[meshIndex];
			mesh.DeduplicateWithinEachSurface();
		}
	}

	private static Rect AdjustRectForEyeExpression(Rect allExpressionsRect, EyeExpression eyeExpression)
	{
		Rect rect = allExpressionsRect;
		rect.width /= 4.0f;
		rect.height /= 2.0f;
		int col = (int)eyeExpression % 4;
		int row = (int)eyeExpression < 4 ? 1 : 0;
		rect.x += rect.width * col;
		rect.y += rect.height * row;
		return rect;
	}

	private int ConvertUnityTransformsToNodesRecursive(Transform currentTransform, ModelNode parentNode)
	{
		// Special case: Inject a new LowerLeg node to improve compatibility with applications
		// that do not properly handle digitigrade legs on bipedal characters.
		if (INJECT_EXTRA_LOWER_LEG && (parentNode.name == "LeftUpperLeg" || parentNode.name == "RightUpperLeg"))
		{
			// Yinglet Creator specific hack: the CompositedYinglet object rotates 180 degrees around Y,
			// resulting in the character being rotated away from the identity. This Quaternion undoes that rotation.
			Vector3 footPos = new Quaternion(0, 1, 0, 0) * currentTransform.GetChild(0).GetChild(0).position;
			ModelNode lowerLeg0 = new ModelNode();
			lowerLeg0.name = ReserveUniqueName(parentNode.name.Replace("Upper", "Lower"));
			lowerLeg0.globalTransformLH = RigidTransform3D.FromUnityGlobalSpin180Y(currentTransform);
			Vector3 tailOffset = footPos - lowerLeg0.globalTransformLH.position;
			Vector3 desiredLocalY = tailOffset.normalized;
			lowerLeg0.boneLength = tailOffset.magnitude;
			// Align the transform's Y axis to point toward the foot.
			lowerLeg0.globalTransformLH.rotation = Quaternion.LookRotation(
				Vector3.Cross(lowerLeg0.globalTransformLH.rotation * Vector3.right, desiredLocalY),
				desiredLocalY
			) * new Quaternion(0, 1, 0, 0);
			lowerLeg0.localTransformLH = RigidTransform3D.RelativeTransform(parentNode.globalTransformLH, lowerLeg0.globalTransformLH);
			lowerLeg0.parent = transformNodeIndexMap[currentTransform.parent];
			// Resume converting and adding children with the new LowerLeg node as the parent.
			int lowerLeg0Index = nodes.Count;
			nodes.Add(lowerLeg0);
			int childIndex = ConvertUnityTransformsToNodesRecursive(currentTransform, lowerLeg0);
			lowerLeg0.children.Add(childIndex);
			nodes[childIndex].parent = lowerLeg0Index;
			return lowerLeg0Index;
		}
		// General case.
		ModelNode selfNode = ModelNode.FromUnityTransform(this, currentTransform, parentNode);
		int selfIndex = nodes.Count;
		nodes.Add(selfNode);
		transformNodeIndexMap[currentTransform] = selfIndex;
		// Special case: Retract the eyes and reset their rotation.
		if (selfNode.name == "LeftEye" || selfNode.name == "RightEye")
		{
			RigidTransform3D globalLH = selfNode.globalTransformLH;
			globalLH.position = new Vector3(globalLH.position.x * -2.0f, globalLH.position.y, globalLH.position.z * -1.0f);
			globalLH.rotation = Quaternion.identity;
			selfNode.globalTransformLH = globalLH;
			selfNode.localTransformLH = RigidTransform3D.RelativeTransform(parentNode.globalTransformLH, globalLH);
		}
		// Recurse into children.
		for (int i = 0; i < currentTransform.childCount; i++)
		{
			Transform childTransform = currentTransform.GetChild(i);
			int childIndex = ConvertUnityTransformsToNodesRecursive(childTransform, selfNode);
			selfNode.children.Add(childIndex);
			nodes[childIndex].parent = selfIndex;
		}
		return selfIndex;
	}

	public Dictionary<int, int> GetNodeIndexToJointMap()
	{
		if (nodeIndexToJointMap != null)
		{
			return nodeIndexToJointMap;
		}
		nodeIndexToJointMap = new Dictionary<int, int>();
		// Prepare a mapping from node index to joint ordinal.
		ModelNode skeletonRootNode = nodes[0];
		for (int ord = 0; ord < skeletonRootNode.skeletonJoints.Count; ord++)
		{
			nodeIndexToJointMap[skeletonRootNode.skeletonJoints[ord]] = ord;
		}
		return nodeIndexToJointMap;
	}

	public void SetExplicitBoneLengths(float antennaLength, float earLength)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (i == 0) continue; // Skip root node.
			ModelNode node = nodes[i];
			// Definitely is a bone, so change it from the default value of -1.0.
			node.boneLength = 0.0;
			// Hard-coded heuristics based on what Yinglet Creator generates.
			if (node.children.Count == 1)
			{
				node.boneLength = nodes[node.children[0]].localTransformLH.position.y;
			}
			else if (node.children.Count == 0)
			{
				if (node.name == "Jaw1")
				{
					node.boneLength = node.localTransformLH.position.y * 20.0f;
				}
				if (node.name == "Snout")
				{
					// Use the same length as Jaw1.
					ModelNode jaw1 = nodes[FindNodeIndexByName("Jaw1")];
					node.boneLength = jaw1.localTransformLH.position.y * 20.0f;
				}
				if (node.name.EndsWith("Antenna"))
				{
					node.boneLength = antennaLength;
				}
				else if (node.name.EndsWith("Ear"))
				{
					node.boneLength = earLength;
				}
				else if (node.name.EndsWith("Hair"))
				{
					float neck1Height = nodes[FindNodeIndexByName("Neck1")].globalTransformLH.position.y;
					node.boneLength = node.globalTransformLH.position.y - neck1Height;
				}
				else
				{
					// Finger bones and other "end" bones: assume the same length as the parent (local Y position).
					node.boneLength = node.localTransformLH.position.y;
				}
			}
			else // if (node.children.Count > 1)
			{
				if (node.name == "Hips")
				{
					node.boneLength = node.GetChildByName(this, "Spine").localTransformLH.position.y;
				}
				else if (node.name == "Spine")
				{
					node.boneLength = node.GetChildByName(this, "Spine1").localTransformLH.position.y;
				}
				else if (node.name == "Chest1")
				{
					node.boneLength = node.GetChildByName(this, "Neck").localTransformLH.position.y;
				}
				else if (node.name == "Head")
				{
					node.boneLength = node.localTransformLH.position.y * 2.0f;
				}
				else if (node.name == "LeftFoot" || node.name == "RightFoot")
				{
					node.boneLength = node.globalTransformLH.position.y;
				}
				else if (node.name == "LeftHand")
				{
					node.boneLength = node.GetChildByName(this, "LeftMiddleProximal").localTransformLH.position.y;
				}
				else if (node.name == "RightHand")
				{
					node.boneLength = node.GetChildByName(this, "RightMiddleProximal").localTransformLH.position.y;
				}
				else
				{
					Debug.LogWarning("Warning: Node " + node.name + " has multiple children, bone length is unknown.");
				}
			}
		}
	}

	public void SetRootNodeName(string requestedName)
	{
		if (nodes.Count > 0)
		{
			nodes[0].name = ReserveUniqueName(requestedName);
		}
	}

	public string ReserveUniqueName(string desiredName)
	{
		if (desiredName.Length == 0)
		{
			return ""; // Allow duplicate empty names.
		}
		string uniqueName = desiredName;
		int discriminator = 2;
		while (uniqueNames.Contains(uniqueName))
		{
			uniqueName = desiredName + discriminator.ToString();
			discriminator++;
		}
		uniqueNames.Add(uniqueName);
		return uniqueName;
	}

	public ModelAnimation FindAnimationByName(string name)
	{
		for (int i = 0; i < animations.Count; i++)
		{
			if (animations[i].name == name)
			{
				return animations[i];
			}
		}
		return null;
	}

	public int FindNodeIndexByName(string name)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i].IsIdentifiedByName(name))
			{
				return i;
			}
		}
		return -1;
	}

	public void ExportSpringRigs(JigglePhysics.JiggleRigBuilder jiggleRigBuilder)
	{
		for (int i = 0; i < jiggleRigBuilder.jiggleRigs.Count; i++)
		{
			JigglePhysics.JiggleRigBuilder.JiggleRig rig = jiggleRigBuilder.jiggleRigs[i];
			springRigs.Add(ModelSpringRig.FromJiggleRig(this, rig));
		}
	}

	public void MessEverythingUpForStupidVRM0xRequirements()
	{
		// VRM 0.x requires "Model faces towards -Z direction" and "No local rotation for bone".
		// See https://github.com/vrm-c/vrm-specification/tree/master/specification/0.0
		// This is mutually incompatible with regular glTF standards, thus the function name.
		Quaternion rot180Y = new Quaternion(0, 1, 0, 0);
		for (int i = 0; i < nodes.Count; i++)
		{
			ModelNode node = nodes[i];
			RigidTransform3D globalLH = node.globalTransformLH;
			globalLH.position = rot180Y * globalLH.position;
			globalLH.rotation = Quaternion.identity;
			node.globalTransformLH = globalLH;
			if (node.parent >= 0)
			{
				ModelNode parentNode = nodes[node.parent];
				node.localTransformLH = RigidTransform3D.RelativeTransform(parentNode.globalTransformLH, node.globalTransformLH);
			}
			// Bone length no longer makes sense with zero rotation, since
			// bones point in the local +Y direction, so set length to zero.
			node.boneLength = 0.0;
		}
		// Spin the mesh around too.
		for (int i = 0; i < meshes.Count; i++)
		{
			ModelMesh mesh = meshes[i];
			for (int s = 0; s < mesh.surfaces.Count; s++)
			{
				ModelMesh.ModelMeshSurface surface = mesh.surfaces[s];
				for (int v = 0; v < surface.vertices.Count; v++)
				{
					surface.vertices[v] = rot180Y * surface.vertices[v];
				}
			}
		}
	}

	/// <summary>
	/// Performs optional final cleanups on the model. This must run AFTER most of the other export steps are done.
	/// </summary>
	public void PerformOptionalCleanups()
	{
		// Round very small local transform values to zero to avoid lots of 1e-7 noise in the exported model.
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes[i].localTransformLH = nodes[i].localTransformLH.RoundToZero();
		}
#pragma warning disable CS0162 // Unreachable code detected
		// If we aren't injecting extra LowerLeg nodes, decrement the number in the mapping.
		// The RCSF standard requires that `LeftLowerLeg1` has a parent named `LeftLowerLeg`.
		// Note: this has to be done after the names were used for other purposes.
		if (!INJECT_EXTRA_LOWER_LEG)
		{
			nodes[FindNodeIndexByName("LeftLowerLeg1")].name = ReserveUniqueName("LeftLowerLeg");
			nodes[FindNodeIndexByName("RightLowerLeg1")].name = ReserveUniqueName("RightLowerLeg");
			nodes[FindNodeIndexByName("LeftLowerLeg2")].name = "LeftLowerLeg1"; // Already reserved.
			nodes[FindNodeIndexByName("RightLowerLeg2")].name = "RightLowerLeg1"; // Already reserved.
		}
#pragma warning restore CS0162 // Unreachable code detected
	}

	public void EncodeMeshDataIntoAccessors(ModelBaseFormat format)
	{
		foreach (ModelMesh mesh in meshes)
		{
			mesh.EncodeMeshDataIntoAccessors(this, format);
		}
	}

	public void EncodeAnimationAccessors(ModelBaseFormat format)
	{
		for (int i = 0; i < animations.Count; i++)
		{
			animations[i].EncodeZeroTimeAnimationAccessors(this, format);
		}
	}

	public void EncodeTextures(int imageFormat)
	{
		for (int i = 0; i < materials.Count; i++)
		{
			ModelMaterial material = materials[i];
			if (material.unityTexture == null)
			{
				Debug.LogError("Material " + material.name + " has no texture to export.");
				continue;
			}
			int textureIndex = ModelTexture.FromMaterial(this, material, imageFormat);
			material.textureIndex = textureIndex;
		}
	}

	public void EncodeThumbnail(Texture2D thumbnailTexture, int imageFormat)
	{
		if (thumbnailTexture == null)
		{
			Debug.LogError("No thumbnail texture provided for export.");
			return;
		}
		thumbnailTextureAndImageIndex = ModelTexture.FromUnityTexture2D(this, thumbnailTexture, "thumbnail", imageFormat);
	}

	public void ExportToG3MF(string path)
	{
		bool isTextFormat = path.EndsWith("tf");
		StringBuilder json = new StringBuilder();
		// Asset header.
		json.Append("{\"asset\":{\"dimension\":3");
		json.Append(",\"extensions\":{\"KHR_xmp_json_ld\":{\"packet\":0}}"); // Packet applies to asset header / entire file.
		json.Append(",\"extensionsUsed\":[");
		// Khronos extension declarations.
		json.Append("\"KHR_character\",\"KHR_character_expression\",\"KHR_character_skeleton_mapping\"");
		json.Append(",\"KHR_materials_unlit\",\"KHR_texture_transform\",\"KHR_xmp_json_ld\"");
		// VRM extension declarations.
		//json.Append(",\"VRMC_materials_mtoon\"");
		json.Append(",\"VRMC_springBone\",\"VRMC_vrm\"");
		json.Append("]"); // End extensionsUsed.
		json.Append(",\"generator\":\"");
		json.Append(GetGeneratorText());
		json.Append("\",\"specification\":\"https://github.com/godot-dimensions/g4mf\"");
		if (thumbnailTextureAndImageIndex >= 0)
		{
			json.Append(",\"thumbnail\":");
			json.Append(thumbnailTextureAndImageIndex);
		}
		// End asset header.
		json.Append("}");
		// Accessors.
		ExportCollectionToJSON(json, "accessors", accessors, ModelBaseFormat.G3MF);
		// Animations.
		json.Append(",\"animation\":{");
		json.Append("\"clips\":[");
		for (int i = 0; i < animations.Count; i++)
		{
			if (i > 0)
			{
				json.Append(",");
			}
			json.Append(animations[i].ModelItemToJSON(ModelBaseFormat.G3MF));
		}
		json.Append("]");
		json.Append("}");
		// Buffers and buffer views.
		EncodeBuffers(json, isTextFormat);
		ExportCollectionToJSON(json, "bufferViews", bufferViews, ModelBaseFormat.G3MF);
		// Top-level extensions.
		json.Append(",\"extensions\":{");
		json.Append("\"KHR_character\":{\"rootNode\":0},");
		ExportKhrCharacterExpressionExtension(json);
		ExportKhrCharacterSkeletonMappingExtension(json);
		ExportDublinCoreKhrXmpJsonLdExtension(json, ModelBaseFormat.GLTF, isTextFormat);
		ExportVRM10TopLevelExtensions(json);
		json.Append("}");
		ExportCollectionToJSON(json, "materials", materials, ModelBaseFormat.G3MF);
		ExportCollectionToJSON(json, "meshes", meshes, ModelBaseFormat.G3MF);
		ExportCollectionToJSON(json, "nodes", nodes, ModelBaseFormat.G3MF);
		ExportCollectionToJSON(json, "textures", textures, ModelBaseFormat.G3MF);
		json.Append("}");
		string jsonString = json.ToString();
		if (isTextFormat)
		{
			// Serialize in the text format (.g3tf)
			System.IO.File.WriteAllText(path, jsonString);
		}
		else
		{
			// Serialize in the binary format (.g3b)
			byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
			int jsonChunkLength = jsonBytes.Length;
			// Pad the JSON chunk to a multiple of 16 bytes using spaces (0x20).
			if (jsonBytes.Length % 16 != 0)
			{
				int padding = 16 - (jsonBytes.Length % 16);
				byte[] paddedJsonBytes = new byte[jsonBytes.Length + padding];
				System.Buffer.BlockCopy(jsonBytes, 0, paddedJsonBytes, 0, jsonBytes.Length);
				for (int i = jsonBytes.Length; i < paddedJsonBytes.Length; i++)
				{
					paddedJsonBytes[i] = 0x20; // Space character for JSON padding.
				}
				jsonBytes = paddedJsonBytes;
			}
			// Write the binary G3B file.
			ulong fileSize = 16ul + 16ul + (ulong)jsonBytes.Length + 16ul + (ulong)bufferData.Count;
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
			using (var bw = new System.IO.BinaryWriter(fs, Encoding.UTF8, leaveOpen: false))
			{
				// G3MF is the 3D profile of the G4MF specification, so it uses "G4MF" in the file header magic.
				// G3B file header
				bw.Write(0x464D3447u); // 'G4MF'
				bw.Write(0); // Version 0
				bw.Write(fileSize); // 64-bit total length / file size
									// JSON chunk
				bw.Write(0x4E4F534Au); // 'JSON'
				bw.Write(0); // Compression format indicator: 0 means uncompressed
				bw.Write((ulong)jsonBytes.Length); // 64-bit chunk length
				bw.Write(jsonBytes);
				// BLOB chunk
				bw.Write(0x424F4C42u); // 'BLOB'
				bw.Write(0); // Compression format indicator: 0 means uncompressed
				bw.Write((ulong)bufferData.Count); // 64-bit chunk length
				bw.Write(bufferData.ToArray());
				bw.Close();
				fs.Close();
			}
		}
	}

	public void ExportToGLTF(string path, int vrmVersion = 1)
	{
		int invBindMat = EncodeInverseBindMatrices();
		bool isTextFormat = path.EndsWith("tf");
		StringBuilder json = new StringBuilder();
		// Asset header.
		json.Append("{\"asset\":{");
		json.Append("\"copyright\":\"" + System.DateTime.UtcNow.ToString("yyyy") + " Yinglet Creator\"");
		json.Append(",\"extensions\":{\"KHR_xmp_json_ld\":{\"packet\":0}}"); // Packet applies to asset header / entire file.
		json.Append(",\"generator\":\"");
		json.Append(GetGeneratorText());
		json.Append("\",\"version\":\"2.0\"}");
		// Accessors.
		ExportCollectionToJSON(json, "accessors", accessors, ModelBaseFormat.GLTF);
		// Animations.
		json.Append(",\"animations\":[");
		for (int i = 0; i < animations.Count; i++)
		{
			if (i > 0)
			{
				json.Append(",");
			}
			json.Append(animations[i].ModelItemToJSON(ModelBaseFormat.GLTF));
		}
		json.Append("]");
		// Buffers and buffer views.
		EncodeBuffers(json, isTextFormat);
		ExportCollectionToJSON(json, "bufferViews", bufferViews, ModelBaseFormat.GLTF);
		// Top-level extensions.
		json.Append(",\"extensions\":{");
		json.Append("\"KHR_character\":{\"rootNode\":0},");
		ExportKhrCharacterExpressionExtension(json);
		ExportKhrCharacterSkeletonMappingExtension(json);
		ExportDublinCoreKhrXmpJsonLdExtension(json, ModelBaseFormat.GLTF, isTextFormat);
		if (vrmVersion == 0)
		{
			ExportVRM0xTopLevelExtensions(json);
		}
		else
		{
			ExportVRM10TopLevelExtensions(json);
		}
		json.Append("}");
		if (ModelAccessor.preferredFloatComponentType == "float16")
		{
			json.Append(",\"extensionsRequired\":[\"KHR_accessor_float16\"]");
		}
		json.Append(",\"extensionsUsed\":[\"GODOT_single_root\"");
		// Khronos extension declarations.
		if (ModelAccessor.preferredFloatComponentType == "float16")
		{
			json.Append(",\"KHR_accessor_float16\"");
		}
		json.Append(",\"KHR_animation_pointer\"");
		json.Append(",\"KHR_character\",\"KHR_character_expression\",\"KHR_character_skeleton_mapping\"");
		json.Append(",\"KHR_materials_unlit\",\"KHR_skeleton_bone\",\"KHR_texture_transform\",\"KHR_xmp_json_ld\"");
		// VRM extension declarations.
		if (vrmVersion == 0)
		{
			json.Append(",\"VRM\",\"VRMC_materials_mtoon\"");
		}
		else
		{
			//json.Append(",\"VRMC_materials_mtoon\"");
			json.Append(",\"VRMC_springBone\",\"VRMC_vrm\"");
		}
		json.Append("]"); // End extensionsUsed
						  // Encode images of textures.
		json.Append(",\"images\":[");
		for (int i = 0; i < textures.Count; i++)
		{
			if (i > 0) json.Append(",");
			json.Append("{\"bufferView\":");
			json.Append(textures[i].bufferViewIndex);
			json.Append(",\"mimeType\":\"");
			json.Append(textures[i].imageMimeType);
			json.Append("\",\"name\":\"");
			json.Append(textures[i].imageName);
			json.Append("\"}");
		}
		json.Append("]");
		// Materials, meshes, nodes, scene.
		ExportCollectionToJSON(json, "materials", materials, ModelBaseFormat.GLTF);
		ExportCollectionToJSON(json, "meshes", meshes, ModelBaseFormat.GLTF);
		ExportCollectionToJSON(json, "nodes", nodes, ModelBaseFormat.GLTF);
		json.Append(",\"scene\":0,\"scenes\":[{\"nodes\":[0]}]");
		// Encode skins/skeleton.
		json.Append(",\"skins\":[{");
		json.Append("\"inverseBindMatrices\":");
		json.Append(invBindMat);
		json.Append(",\"joints\":[");
		for (int i = 0; i < nodes[0].skeletonJoints.Count; i++)
		{
			if (i > 0) json.Append(",");
			json.Append(nodes[0].skeletonJoints[i]);
		}
		json.Append("],\"skeleton\":0}]");
		ExportCollectionToJSON(json, "textures", textures, ModelBaseFormat.GLTF);
		json.Append("}");
		string jsonString = json.ToString();
		if (isTextFormat)
		{
			// Serialize in the text format (.gltf)
			System.IO.File.WriteAllText(path, jsonString);
		}
		else
		{
			// Serialize in the binary format (.glb or .vrm)
			byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
			// Pad the JSON chunk to a multiple of 4 bytes using spaces (0x20).
			if (jsonBytes.Length % 4 != 0)
			{
				int padding = 4 - (jsonBytes.Length % 4);
				byte[] paddedJsonBytes = new byte[jsonBytes.Length + padding];
				System.Buffer.BlockCopy(jsonBytes, 0, paddedJsonBytes, 0, jsonBytes.Length);
				for (int i = jsonBytes.Length; i < paddedJsonBytes.Length; i++)
				{
					paddedJsonBytes[i] = 0x20; // Space character for JSON padding.
				}
				jsonBytes = paddedJsonBytes;
			}
			// GLB requires all chunks to be padded, not just the first like G3B.
			if (bufferData.Count % 4 != 0)
			{
				int padding = 4 - (bufferData.Count % 4);
				for (int i = 0; i < padding; i++)
				{
					bufferData.Add(0); // Zero byte for BIN padding.
				}
			}
			// Write the binary GLB/VRM file.
			uint fileSize = 12u + 8u + (uint)jsonBytes.Length + 8u + (uint)bufferData.Count;
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
			using (var bw = new System.IO.BinaryWriter(fs, Encoding.UTF8, leaveOpen: false))
			{
				// GLB file header
				bw.Write(0x46546C67u); // 'glTF'
				bw.Write(2u); // Version 2
				bw.Write(fileSize); // 32-bit total length / file size
									// JSON chunk
				bw.Write((uint)jsonBytes.Length); // 32-bit chunk length
				bw.Write(0x4E4F534Au); // 'JSON'
				bw.Write(jsonBytes);
				// BIN chunk
				bw.Write((uint)bufferData.Count); // 32-bit chunk length
				bw.Write(0x004E4942u); // 'BIN\0'
				bw.Write(bufferData.ToArray());
				bw.Close();
				fs.Close();
			}
		}
	}

	private void ExportDublinCoreKhrXmpJsonLdExtension(StringBuilder json, ModelBaseFormat format, bool isTextFormat)
	{
		json.Append("\"KHR_xmp_json_ld\":{\"packets\":[");
		json.Append("{\"@context\":{\"dc\":\"http://purl.org/dc/elements/1.1/\"},\"@id\":\"\"");
		json.Append(",\"dc:contributor\":{\"@set\":[\"Thomas 'Tabski' Bartlett\",\"Valsalia\"]}");
		json.Append(",\"dc:creator\":{\"@list\":[\"Yinglet Creator\"]}");
		json.Append(",\"dc:date\":\"");
		json.Append(System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
		json.Append("\",\"dc:description\":\"A silly little Yinglet\"");
		json.Append(",\"dc:format\":\"");
		if (format == ModelBaseFormat.GLTF)
		{
			json.Append(isTextFormat ? "model/gltf+json" : "model/gltf-binary");
		}
		else
		{
			// G3MF is the 3D profile of the G4MF specification, so it uses "g4mf" in the MIME type.
			json.Append(isTextFormat ? "model/g4mf+json" : "model/g4mf-binary");
		}
		json.Append("\",\"dc:relation\":{\"@set\":[\"https://store.steampowered.com/app/3954540/Yinglet_Creator/\",\"https://github.com/TBartl/YingletCreator\",\"https://creativecommons.org/licenses/by-nc-sa/4.0/\"]}");
		json.Append(",\"dc:rights\":\"CC BY-NC-SA 4.0\"");
		json.Append(",\"dc:subject\":{\"@set\":[\"Yinglet\",\"Furry\",\"Cartoon\"]}");
		json.Append(",\"dc:title\":\"");
		json.Append(nodes[0].name);
		json.Append("\",\"dc:type\":{\"@set\":[\"Character\"]}");
		json.Append("}]},");
	}

	private void ExportKhrCharacterExpressionExtension(StringBuilder json)
	{
		json.Append("\"KHR_character_expression\":{\"expressions\":[");
		int expressionIndex = 0;
		for (int animIndex = 0; animIndex < animations.Count; animIndex++)
		{
			ModelAnimation animation = animations[animIndex];
			// Yinglet Creator specific hack: This assumes that all non-RESET animations are expressions.
			// If there are non-eye expressions, there should be separate animations for them,
			// which exist alongside the eye-specific ones, and would be used instead here.
			if (animation.name == "RESET") continue;
			string eyeExpressionName = animation.name.Replace("Eyes", "");
			if (expressionIndex > 0) json.Append(",");
			json.Append("{");
			json.AppendFormat("\"animation\":{0}", animIndex);
			json.AppendFormat(",\"expression\":\"{0}\"", eyeExpressionName);
			json.Append("}");
			expressionIndex++;
		}
		json.Append("]},");
	}

	private void ExportKhrCharacterSkeletonMappingExtension(StringBuilder json)
	{
		json.Append("\"KHR_character_skeleton_mapping\":{\"skeletalRigMappings\":{");
		{
			json.Append("\"vrmHumanoid\":{");
			int vrmHumanBoneIndex = 0;
			foreach (var kvp in ModelNode.vrmToStandardRCSF)
			{
				int nodeIndex = FindNodeIndexByName(kvp.Value);
				if (nodeIndex < 0) continue;
				if (vrmHumanBoneIndex > 0) json.Append(",");
				json.AppendFormat("\"{0}\":\"{1}\"", kvp.Key, kvp.Value);
				vrmHumanBoneIndex++;
			}
			json.Append("}");
		}
		// Empty mapping to indicate the node names already follow the RCSF standard.
		json.Append(",\"rcsf\":{}");
		{
			json.Append(",\"unityMecanim\":{");
			int unityMecanimBoneIndex = 0;
			foreach (var kvp in ModelNode.unityMecanimToStandardRCSF)
			{
				int nodeIndex = FindNodeIndexByName(kvp.Value);
				if (nodeIndex < 0) continue;
				if (unityMecanimBoneIndex > 0) json.Append(",");
				json.AppendFormat("\"{0}\":\"{1}\"", kvp.Key, kvp.Value);
				unityMecanimBoneIndex++;
			}
			json.Append("}");
		}
		json.Append("}},");
	}

	/// <summary>
	/// Appends the top-level VRM 0.x extensions to the given JSON StringBuilder.
	/// </summary>
	private void ExportVRM0xTopLevelExtensions(StringBuilder json)
	{
		json.Append("\"VRM\":{");
		json.Append("\"exporterVersion\":\"");
		json.Append(GetGeneratorText());
		json.Append("\"");
		json.Append(",\"firstPerson\":{");
		{
			// Hard-coded values designed to work well with the exported Yinglet eye bones.
			json.Append("\"lookAtHorizontalInner\":{\"xRange\":50.0,\"yRange\":3.0},");
			json.Append("\"lookAtHorizontalOuter\":{\"xRange\":50.0,\"yRange\":6.0},");
			json.Append("\"lookAtTypeName\":\"Bone\",");
			json.Append("\"lookAtVerticalDown\":{\"xRange\":40.0,\"yRange\":3.0},");
			json.Append("\"lookAtVerticalUp\":{\"xRange\":40.0,\"yRange\":3.0}");
		}
		json.Append("},\"humanoid\":{");
		{
			json.Append("\"humanBones\":[");
			int vrmHumanBoneIndex = 0;
			foreach (var kvp in ModelNode.vrmToStandardRCSF)
			{
				int nodeIndex = FindNodeIndexByName(kvp.Value);
				if (nodeIndex < 0) continue;
				if (vrmHumanBoneIndex > 0) json.Append(",");
				string boneName = kvp.Key;
				// VRM 0.x uses medically incorrect terminology for the thumb bones,
				// so this re-breaks what VRM 1.0 fixed, for compatibility.
				if (boneName == "leftThumbProximal") boneName = "leftThumbIntermediate";
				else if (boneName == "rightThumbProximal") boneName = "rightThumbIntermediate";
				else if (boneName == "leftThumbMetacarpal") boneName = "leftThumbProximal";
				else if (boneName == "rightThumbMetacarpal") boneName = "rightThumbProximal";
				json.AppendFormat("{{\"bone\":\"{0}\",\"node\":{1}}}", boneName, nodeIndex);
				vrmHumanBoneIndex++;
			}
			json.Append("]");
		}
		json.Append("},\"materialProperties\":[");
		{
			for (int i = 0; i < materials.Count; i++)
			{
				if (i > 0) json.Append(",");
				json.Append("{\"name\":\"");
				json.Append(materials[i].name);
				json.Append("\",\"shader\":\"VRM_USE_GLTFSHADER\"}");
			}
		}
		json.Append("],\"meta\":{");
		{
			json.Append("\"allowedUserName\":\"Everyone\",");
			json.Append("\"author\":\"Yinglet Creator\",");
			json.Append("\"commercialUssageName\":\"Disallow\",");
			json.Append("\"licenseName\":\"CC_BY_NC_SA\",");
			json.Append("\"otherLicenseUrl\":\"https://github.com/TBartl/YingletCreator/blob/master/LICENSE\",");
			json.Append("\"sexualUssageName\":\"Allow\",");
			if (thumbnailTextureAndImageIndex >= 0)
			{
				json.Append("\"texture\":"); // VRM 0.x has the dumbest names I swear.
				json.Append(thumbnailTextureAndImageIndex);
				json.Append(",");
			}
			json.Append("\"title\":\"");
			json.Append(nodes[0].name);
			json.Append("\",");
			json.Append("\"version\":\"0.0\",");
			json.Append("\"violentUssageName\":\"Allow\"");
		}
		json.Append("},\"secondaryAnimation\":{");
		{
			json.Append("\"boneGroups\":[");
			for (int i = 0; i < springRigs.Count; i++)
			{
				if (i > 0) json.Append(",");
				json.Append(springRigs[i].ToVRM0xJSON());
			}
			json.Append("]");
		}
		json.Append("},\"specVersion\":\"0.0\"");
		json.Append("}");
	}

	/// <summary>
	/// Appends the top-level VRM 1.0 extensions to the given JSON StringBuilder.
	/// </summary>
	private void ExportVRM10TopLevelExtensions(StringBuilder json)
	{
		json.Append("\"VRMC_springBone\":{");
		{
			json.Append("\"specVersion\":\"1.0\",");
			json.Append("\"springs\":[");
			for (int i = 0; i < springRigs.Count; i++)
			{
				if (i > 0) json.Append(",");
				// Note: All ModelItemToJSON functions require the base format but it's unused here.
				json.Append(springRigs[i].ModelItemToJSON(ModelBaseFormat.GLTF));
			}
			json.Append("]"); // End springs
		}
		json.Append("},\"VRMC_vrm\":{");
		{
			json.Append("\"expressions\":{");
			{
				json.Append("\"custom\":{");
				ModelAnimation winceAnim = FindAnimationByName("EyesWince");
				AnimationToVRM10ExpressionJSON(json, winceAnim, "wince");
				json.Append("},\"preset\":{");
				int expressionIndex = 0;
				foreach (var kvp in vrmExpressionToAnimationName)
				{
					ModelAnimation animation = FindAnimationByName(kvp.Value);
					if (expressionIndex > 0) json.Append(",");
					AnimationToVRM10ExpressionJSON(json, animation, kvp.Key);
					expressionIndex++;
				}
				json.Append("}");
			}
			json.Append("},\"humanoid\":{");
			{
				json.Append("\"humanBones\":{");
				int vrmHumanBoneIndex = 0;
				foreach (var kvp in ModelNode.vrmToStandardRCSF)
				{
					int nodeIndex = FindNodeIndexByName(kvp.Value);
					if (nodeIndex < 0) continue;
					if (vrmHumanBoneIndex > 0) json.Append(",");
					json.AppendFormat("\"{0}\":{{\"node\":{1}}}", kvp.Key, nodeIndex);
					vrmHumanBoneIndex++;
				}
				json.Append("}");
			}
			json.Append("},\"lookAt\":{");
			{
				// Hard-coded values designed to work well with the exported Yinglet eye bones.
				json.Append("\"rangeMapHorizontalInner\":{\"inputMaxValue\":50.0,\"outputScale\":3.0},");
				json.Append("\"rangeMapHorizontalOuter\":{\"inputMaxValue\":50.0,\"outputScale\":6.0},");
				json.Append("\"rangeMapVerticalDown\":{\"inputMaxValue\":40.0,\"outputScale\":3.0},");
				json.Append("\"rangeMapVerticalUp\":{\"inputMaxValue\":40.0,\"outputScale\":3.0},");
				json.Append("\"type\":\"bone\"");
			}
			json.Append("},\"meta\":{");
			{
				json.Append("\"allowAntisocialOrHateUsage\":true,");
				json.Append("\"allowExcessivelySexualUsage\":true,");
				json.Append("\"allowExcessivelyViolentUsage\":true,");
				json.Append("\"allowPoliticalOrReligiousUsage\":true,");
				json.Append("\"allowRedistribution\":true,");
				json.Append("\"authors\":[\"Yinglet Creator\"],");
				json.Append("\"avatarPermission\":\"everyone\",");
				json.Append("\"commercialUsage\":\"personalNonProfit\",");
				json.Append("\"copyrightInformation\":\"Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International\",");
				json.Append("\"creditNotation\":\"unnecessary\",");
				json.Append("\"licenseUrl\":\"https://vrm.dev/licenses/1.0/\",");
				json.Append("\"modification\":\"allowModificationRedistribution\",");
				json.Append("\"name\":\"");
				json.Append(nodes[0].name);
				json.Append("\",");
				json.Append("\"otherLicenseUrl\":\"https://github.com/TBartl/YingletCreator/blob/master/LICENSE\",");
				if (thumbnailTextureAndImageIndex >= 0)
				{
					json.Append("\"thumbnailImage\":");
					json.Append(thumbnailTextureAndImageIndex);
					json.Append(",");
				}
				json.Append("\"version\":\"0.0\"");
			}
			json.Append("},");
			json.Append("\"specVersion\":\"1.0\"");
		}
		json.Append("}");
	}

	private void AnimationToVRM10ExpressionJSON(StringBuilder json, ModelAnimation animation, string vrmName)
	{
		json.Append("\"");
		json.Append(vrmName);
		json.Append("\":{\"isBinary\":true,\"textureTransformBinds\":[{");
		json.Append("\"material\":");
		json.Append(animation.targetMaterialIndex);
		json.Append(",\"offset\":[");
		json.Append(ModelItem.Flt(animation.offset.x));
		json.Append(",");
		json.Append(ModelItem.Flt(animation.offset.y));
		json.Append("]"); // Close offset
		json.Append(",\"scale\":[1.0,1.0]"); // HACK: UniVRM crashes when scale is missing, even though it's optional in the spec.
		json.Append("}]}"); // Close textureTransformBinds, close expression.
	}

	private void ExportCollectionToJSON<T>(StringBuilder json, string name, List<T> items, ModelBaseFormat baseFormat) where T : ModelItem
	{
		json.Append(",\"" + name + "\":[");
		for (int i = 0; i < items.Count; i++)
		{
			if (i > 0) json.Append(",");
			json.Append(items[i].ModelItemToJSON(baseFormat));
		}
		json.Append("]");
	}

	private void EncodeBuffers(StringBuilder json, bool isTextFormat)
	{
		json.Append(",\"buffers\":[{\"byteLength\":");
		json.Append(bufferData.Count);
		if (isTextFormat)
		{
			json.Append(",\"uri\":\"data:application/octet-stream;base64,");
			json.Append(System.Convert.ToBase64String(bufferData.ToArray()));
			json.Append("\"");
		}
		json.Append("}]");
	}

	private int EncodeInverseBindMatrices()
	{
		List<Matrix4x4> inverseBindMatrices = new List<Matrix4x4>();
		ModelNode skeleton = nodes[0];
		Matrix4x4 meshWorldBind = nodes[nodes.Count - 1].globalTransformLH.ToMatrix4x4();
		for (int i = 0; i < skeleton.skeletonJoints.Count; i++)
		{
			int jointIndex = skeleton.skeletonJoints[i];
			ModelNode jointNode = nodes[jointIndex];
			Matrix4x4 jointWorldBind = jointNode.globalTransformLH.ToRightHanded().ToMatrix4x4();
			Matrix4x4 ibm = jointWorldBind.inverse * meshWorldBind;
			inverseBindMatrices.Add(ibm);
		}
		return ModelAccessor.EncodeMatrix4x4s(this, inverseBindMatrices);
	}

	private static string GetGeneratorText()
	{
		return "Yinglet Creator " + Application.version;
	}

	/// <summary>
	/// VRM 1.0's expression names have a decent mapping for everything except "EyesWince".
	/// </summary>
	private static readonly Dictionary<string, string> vrmExpressionToAnimationName = new Dictionary<string, string>
	{
		{"angry", "EyesAngry"},
		{"blink", "EyesBlink"},
		{"happy", "EyesPleased"},
		{"neutral", "EyesNeutral"},
		{"relaxed", "EyesSquint"},
		{"sad", "EyesSad"},
		{"surprised", "EyesShocked"},
	};
}
