using System.Collections.Generic;
using UnityEngine;

public class ModelNode : ModelItem
{
	/// <summary>
	/// If non-negative, the length of the bone in the local +Y direction.
	/// </summary>
	public double boneLength = -1.0;
	/// <summary>
	/// The indices of child nodes in the ModelDocument's node list.
	/// </summary>
	public List<int> children = new List<int>();
	/// <summary>
	/// The index of the parent node in the ModelDocument's node list, or -1 if this is the root node. This is not exported, just a helper.
	/// </summary>
	public int parent = -1;
	/// <summary>
	/// If this node is a mesh node, the index of the mesh in the ModelDocument's mesh list.
	/// </summary>
	public int mesh = -1;
	/// <summary>
	/// If this node is a skeleton root, the list of joint node indices in the ModelDocument's node list.
	/// </summary>
	public List<int> skeletonJoints = null;
	/// <summary>
	/// The position and rotation of this node in the global coordinate system in left-handed Unity space. This is not exported, just a helper.
	/// </summary>
	public RigidTransform3D globalTransformLH = RigidTransform3D.Identity;
	/// <summary>
	/// The position and rotation of this node in its parent's coordinate system in left-handed Unity space.
	/// </summary>
	public RigidTransform3D localTransformLH = RigidTransform3D.Identity;
	/// <summary>
	/// The original Unity Transform this node was created from. This is not exported, just a helper.
	/// </summary>
	public Transform unityTransform;

	public static ModelNode FromUnityTransform(ModelDocument doc, Transform fromTransform, ModelNode parentNode)
	{
		ModelNode node = new ModelNode();
		// Remap Yinglet Creator bone names to standard RCSF bone names.
		if (_yingletCreatorToStandardRCSF.TryGetValue(fromTransform.name, out string standardName))
		{
			node.name = standardName;
		}
		else
		{
			node.name = fromTransform.name;
		}
		node.name = doc.ReserveUniqueName(node.name);
		node.unityTransform = fromTransform;
		// Yinglet Creator specific hack: the CompositedYinglet object rotates 180 degrees around Y,
		// resulting in the character being rotated away from the identity. This function undoes that rotation.
		node.globalTransformLH = RigidTransform3D.FromUnityGlobalSpin180Y(fromTransform);
		if (yingletRcsfBonesToCorrect.Contains(node.name))
		{
			node.globalTransformLH.rotation = node.globalTransformLH.rotation * new Quaternion(0, 1, 0, 0);
		}
		node.localTransformLH = RigidTransform3D.RelativeTransform(parentNode.globalTransformLH, node.globalTransformLH);
		return node;
	}

	public ModelNode GetChildByName(ModelDocument doc, string childName)
	{
		foreach (int childIndex in children)
		{
			ModelNode childNode = doc.nodes[childIndex];
			if (childNode.IsIdentifiedByName(childName))
			{
				return childNode;
			}
		}
		return null;
	}

	public bool IsIdentifiedByName(string queryName)
	{
		if (name == queryName) return true;
		if (_yingletCreatorToStandardRCSF.TryGetValue(queryName, out string rcsfToStandardizedQueryName))
		{
			if (name == rcsfToStandardizedQueryName) return true;
		}
		if (vrmToStandardRCSF.TryGetValue(queryName, out string vrmToStandardizedQueryName))
		{
			if (name == vrmToStandardizedQueryName) return true;
		}
		return false;
	}

	private string WriteBoneJSON()
	{
		if (boneLength > 0.0)
		{
			return "{\"length\":" + Flt(boneLength) + "}";
		}
		return "{}";
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{");
		if (boneLength >= 0.0 && format == ModelBaseFormat.G3MF)
		{
			json.Append("\"bone\":" + WriteBoneJSON() + ",");
		}
		if (children.Count > 0)
		{
			json.Append("\"children\":[");
			for (int i = 0; i < children.Count; i++)
			{
				if (i > 0) json.Append(",");
				json.Append(children[i]);
			}
			json.Append("],");
		}
		if (boneLength >= 0.0 && format == ModelBaseFormat.GLTF)
		{
			json.Append("\"extensions\":{\"KHR_skeleton_bone\":" + WriteBoneJSON() + "},");
		}
		if (mesh >= 0)
		{
			json.Append("\"mesh\":" + mesh + ",");
		}
		json.Append("\"name\":\"" + name + "\"");
		RigidTransform3D localTransformRH = localTransformLH.ToRightHanded();
		if (format == ModelBaseFormat.GLTF)
		{
			// Convert from Unity left-handed to glTF right-handed by negating the X position and negating the Quaternion's Y and Z parts.
			if (localTransformRH.rotation != Quaternion.identity)
			{
				json.AppendFormat(",\"rotation\":[{0},{1},{2},{3}]", Flt(localTransformRH.rotation.x), Flt(localTransformRH.rotation.y), Flt(localTransformRH.rotation.z), Flt(localTransformRH.rotation.w));
			}
			if (localTransformRH.position != Vector3.zero)
			{
				json.AppendFormat(",\"translation\":[{0},{1},{2}]", Flt(localTransformRH.position.x), Flt(localTransformRH.position.y), Flt(localTransformRH.position.z));
			}
		}
		else
		{
			// Write skeleton joints if any.
			if (skeletonJoints != null && skeletonJoints.Count > 0)
			{
				json.Append(",\"skeleton\":{\"joints\":[");
				for (int i = 0; i < skeletonJoints.Count; i++)
				{
					if (i > 0) json.Append(",");
					json.Append(skeletonJoints[i]);
				}
				json.Append("]}");
			}
			// Convert from Unity left-handed to G3MF right-handed by negating the X position, moving the real part of the Quaternion to
			// the front of the rotor, and negating the Z part of the rotation (X and Y get double negated and thus stay the same).
			if (localTransformRH.position != Vector3.zero)
			{
				json.AppendFormat(",\"position\":[{0},{1},{2}]", Flt(localTransformRH.position.x), Flt(localTransformRH.position.y), Flt(localTransformRH.position.z));
			}
			if (localTransformRH.rotation != Quaternion.identity)
			{
				// This is a Geometric Algebra rotor: https://github.com/godot-dimensions/g4mf/blob/main/specification/parts/node.md#rotor
				json.AppendFormat(",\"rotor\":[{0},{1},{2},{3}]", Flt(localTransformRH.rotation.w), Flt(localTransformRH.rotation.z), Flt(-localTransformRH.rotation.y), Flt(localTransformRH.rotation.x));
			}
		}
		if (mesh >= 0 && format == ModelBaseFormat.GLTF)
		{
			json.Append(",\"skin\":0"); // Hard-code a single skin for simplicity.
		}
		json.Append("}");
		return json.ToString();
	}

	/// <summary>
	/// Standard bone names designed to be portable between applications. See https://github.com/meshula/LabRCSF
	/// </summary>
	private static readonly Dictionary<string, string> _yingletCreatorToStandardRCSF = new Dictionary<string, string>
	{
		{"root", "Hips"},
		{"spine", "Spine"},
		{"belly", "Belly"},
		{"spine.001", "Spine1"},
		{"spine.002", "Chest"},
		{"spine.003", "Chest1"},
		{"breast.L", "LeftBreast"},
		{"breast.R", "RightBreast"},
		{"shoulder.L", "LeftShoulder"},
		{"upper_arm.L", "LeftUpperArm"},
		{"forearm.L", "LeftLowerArm"},
		{"hand.L", "LeftHand"},
		{"f_index.01.L", "LeftIndexProximal"},
		{"f_index.02.L", "LeftIndexIntermediate"},
		{"f_index.03.L", "LeftIndexDistal"},
		{"f_middle.01.L", "LeftMiddleProximal"},
		{"f_middle.02.L", "LeftMiddleIntermediate"},
		{"f_middle.03.L", "LeftMiddleDistal"},
		{"f_ring.01.L", "LeftRingProximal"},
		{"f_ring.02.L", "LeftRingIntermediate"},
		{"f_ring.03.L", "LeftRingDistal"},
		{"thumb.01.L", "LeftThumbMetacarpal"},
		{"thumb.02.L", "LeftThumbProximal"},
		{"thumb.03.L", "LeftThumbDistal"},
		{"shoulder.R", "RightShoulder"},
		{"upper_arm.R", "RightUpperArm"},
		{"forearm.R", "RightLowerArm"},
		{"hand.R", "RightHand"},
		{"f_index.01.R", "RightIndexProximal"},
		{"f_index.02.R", "RightIndexIntermediate"},
		{"f_index.03.R", "RightIndexDistal"},
		{"f_middle.01.R", "RightMiddleProximal"},
		{"f_middle.02.R", "RightMiddleIntermediate"},
		{"f_middle.03.R", "RightMiddleDistal"},
		{"f_ring.01.R", "RightRingProximal"},
		{"f_ring.02.R", "RightRingIntermediate"},
		{"f_ring.03.R", "RightRingDistal"},
		{"thumb.01.R", "RightThumbMetacarpal"},
		{"thumb.02.R", "RightThumbProximal"},
		{"thumb.03.R", "RightThumbDistal"},
		{"spine.004", "Neck"},
		{"spine.005", "Neck1"},
		{"face", "Head"},
		{"antenna_L", "LeftAntenna"},
		{"antenna_R", "RightAntenna"},
		{"ear_L", "LeftEar"},
		{"ear_R", "RightEar"},
		{"eye_L", "LeftEye"},
		{"eye_R", "RightEye"},
		{"hair.back_L", "LeftRearHair"},
		{"hair.back_R", "RightRearHair"},
		{"hair.front_L", "LeftFrontHair"},
		{"hair.front_R", "RightFrontHair"},
		{"hat", "Hat"},
		{"mandible", "Jaw"},
		{"snout_lower", "Jaw1"},
		{"nose_bridge", "NoseBridge"},
		{"snout", "Snout"},
		{"tail.001", "Tail"},
		{"tail.002", "Tail1"},
		{"tail.003", "Tail2"},
		{"tail.004", "Tail3"},
		{"tail.005", "Tail4"},
		{"tail.006", "Tail5"},
		{"tail.007", "Tail6"},
		{"tail.008", "Tail7"},
		{"thigh.L", "LeftUpperLeg"},
		{"shin.L", "LeftLowerLeg1"}, // Special case: See note in `ModelDocument.ConvertUnityTransformsToNodesRecursive`.
		{"foot.L", "LeftLowerLeg2"},
		{"toe.L", "LeftFoot"},
		{"foot_index.01.L", "LeftIndexToeProximal"},
		{"foot_index.02.L", "LeftIndexToeIntermediate"},
		{"foot_index.03.L", "LeftIndexToeDistal"},
		{"foot_middle.01.L", "LeftMiddleToeProximal"},
		{"foot_middle.02.L", "LeftMiddleToeIntermediate"},
		{"foot_middle.03.L", "LeftMiddleToeDistal"},
		{"foot_ring.01.L", "LeftRingToeProximal"},
		{"foot_ring.02.L", "LeftRingToeIntermediate"},
		{"foot_ring.03.L", "LeftRingToeDistal"},
		{"foot_thumb.01.L", "LeftHalluxMetacarpal"},
		{"foot_thumb.02.L", "LeftHalluxProximal"},
		{"foot_thumb.03.L", "LeftHalluxDistal"},
		{"thigh.R", "RightUpperLeg"},
		{"shin.R", "RightLowerLeg1"}, // Special case: See note in `ModelDocument.ConvertUnityTransformsToNodesRecursive`.
		{"foot.R", "RightLowerLeg2"},
		{"toe.R", "RightFoot"},
		{"foot_index.01.R", "RightIndexToeProximal"},
		{"foot_index.02.R", "RightIndexToeIntermediate"},
		{"foot_index.03.R", "RightIndexToeDistal"},
		{"foot_middle.01.R", "RightMiddleToeProximal"},
		{"foot_middle.02.R", "RightMiddleToeIntermediate"},
		{"foot_middle.03.R", "RightMiddleToeDistal"},
		{"foot_ring.01.R", "RightRingToeProximal"},
		{"foot_ring.02.R", "RightRingToeIntermediate"},
		{"foot_ring.03.R", "RightRingToeDistal"},
		{"foot_thumb.01.R", "RightHalluxMetacarpal"},
		{"foot_thumb.02.R", "RightHalluxProximal"},
		{"foot_thumb.03.R", "RightHalluxDistal"},
	};

	/// <summary>
	/// Mapping from VRM 1.0 humanoid bone names to standard RCSF bone names.
	/// </summary>
	public static readonly Dictionary<string, string> vrmToStandardRCSF = new Dictionary<string, string>
	{
		{"chest", "Chest"},
		{"head", "Head"},
		{"hips", "Hips"},
		{"jaw", "Jaw"},
		{"leftEye", "LeftEye"},
		{"leftFoot", "LeftFoot"},
		{"leftHand", "LeftHand"},
		{"leftIndexDistal", "LeftIndexDistal"},
		{"leftIndexIntermediate", "LeftIndexIntermediate"},
		{"leftIndexProximal", "LeftIndexProximal"},
		{"leftLittleDistal", "LeftPinkyDistal"},
		{"leftLittleIntermediate", "LeftPinkyIntermediate"},
		{"leftLittleProximal", "LeftPinkyProximal"},
		{"leftLowerArm", "LeftLowerArm"},
		{"leftLowerLeg", "LeftLowerLeg"},
		{"leftMiddleDistal", "LeftMiddleDistal"},
		{"leftMiddleIntermediate", "LeftMiddleIntermediate"},
		{"leftMiddleProximal", "LeftMiddleProximal"},
		{"leftRingDistal", "LeftRingDistal"},
		{"leftRingIntermediate", "LeftRingIntermediate"},
		{"leftRingProximal", "LeftRingProximal"},
		{"leftShoulder", "LeftShoulder"},
		{"leftThumbDistal", "LeftThumbDistal"},
		{"leftThumbMetacarpal", "LeftThumbMetacarpal"},
		{"leftThumbProximal", "LeftThumbProximal"},
		{"leftToes", "LeftToes"},
		{"leftUpperArm", "LeftUpperArm"},
		{"leftUpperLeg", "LeftUpperLeg"},
		{"neck", "Neck"},
		{"rightEye", "RightEye"},
		{"rightFoot", "RightFoot"},
		{"rightHand", "RightHand"},
		{"rightIndexDistal", "RightIndexDistal"},
		{"rightIndexIntermediate", "RightIndexIntermediate"},
		{"rightIndexProximal", "RightIndexProximal"},
		{"rightLittleDistal", "RightPinkyDistal"},
		{"rightLittleIntermediate", "RightPinkyIntermediate"},
		{"rightLittleProximal", "RightPinkyProximal"},
		{"rightLowerArm", "RightLowerArm"},
		{"rightLowerLeg", "RightLowerLeg"},
		{"rightMiddleDistal", "RightMiddleDistal"},
		{"rightMiddleIntermediate", "RightMiddleIntermediate"},
		{"rightMiddleProximal", "RightMiddleProximal"},
		{"rightRingDistal", "RightRingDistal"},
		{"rightRingIntermediate", "RightRingIntermediate"},
		{"rightRingProximal", "RightRingProximal"},
		{"rightShoulder", "RightShoulder"},
		{"rightThumbDistal", "RightThumbDistal"},
		{"rightThumbMetacarpal", "RightThumbMetacarpal"},
		{"rightThumbProximal", "RightThumbProximal"},
		{"rightToes", "RightToes"},
		{"rightUpperArm", "RightUpperArm"},
		{"rightUpperLeg", "RightUpperLeg"},
		{"spine", "Spine"},
		{"upperChest", "Chest1"},
	};

	/// <summary>
	/// Mapping from Unity Mecanim humanoid bone names to standard RCSF bone names.
	/// </summary>
	public static readonly Dictionary<string, string> unityMecanimToStandardRCSF = new Dictionary<string, string>
	{
		{"LeftLittleDistal", "LeftPinkyDistal"},
		{"LeftLittleIntermediate", "LeftPinkyIntermediate"},
		{"LeftLittleProximal", "LeftPinkyProximal"},
		{"LeftThumbIntermediate", "LeftThumbProximal"},
		{"LeftThumbProximal", "LeftThumbMetacarpal"},
		{"RightLittleDistal", "RightPinkyDistal"},
		{"RightLittleIntermediate", "RightPinkyIntermediate"},
		{"RightLittleProximal", "RightPinkyProximal"},
		{"RightThumbIntermediate", "RightThumbProximal"},
		{"RightThumbProximal", "RightThumbMetacarpal"},
		{"UpperChest", "Chest1"},
	};

	/// <summary>
	/// Some of Yinglet Creator's bones do not match the standard bone roll, this corrects them.
	/// </summary>
	private static readonly List<string> yingletRcsfBonesToCorrect = new List<string>
	{
		"LeftHand",
		"LeftIndexDistal",
		"LeftIndexIntermediate",
		"LeftIndexProximal",
		"LeftLowerArm",
		"LeftLowerArm",
		"LeftLowerLeg1",
		"LeftLowerLeg2",
		"LeftMiddleDistal",
		"LeftMiddleIntermediate",
		"LeftMiddleProximal",
		"LeftPinkyDistal",
		"LeftPinkyIntermediate",
		"LeftPinkyProximal",
		"LeftRingDistal",
		"LeftRingIntermediate",
		"LeftRingProximal",
		"LeftShoulder",
		"LeftThumbDistal",
		"LeftThumbMetacarpal",
		"LeftThumbProximal",
		"LeftUpperArm",
		"LeftUpperLeg",
		"RightHand",
		"RightIndexDistal",
		"RightIndexIntermediate",
		"RightIndexProximal",
		"RightLowerArm",
		"RightLowerArm",
		"RightLowerLeg1",
		"RightLowerLeg2",
		"RightMiddleDistal",
		"RightMiddleIntermediate",
		"RightMiddleProximal",
		"RightPinkyDistal",
		"RightPinkyIntermediate",
		"RightPinkyProximal",
		"RightRingDistal",
		"RightRingIntermediate",
		"RightRingProximal",
		"RightShoulder",
		"RightThumbDistal",
		"RightThumbMetacarpal",
		"RightThumbProximal",
		"RightUpperArm",
		"RightUpperLeg",
	};
}
