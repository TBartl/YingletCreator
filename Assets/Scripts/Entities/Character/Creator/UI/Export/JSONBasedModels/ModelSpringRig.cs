using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class sits in-between Naelstrof's JigglePhysics.JiggleRig and the exported VRM spring bone format.
/// </summary>
public class ModelSpringRig : ModelItem
{
	public JigglePhysics.JiggleRigBuilder.JiggleRig jiggleRig;
	public JigglePhysics.JiggleSettingsData jiggleSettingsData;
	public List<int> jointNodeIndices = new List<int>();
	public float dragForce = 0.5f;
	public float gravityPower = 0.0f;
	public float gravityY = -1.0f;
	public float stiffness = 1.0f;

	public static ModelSpringRig FromJiggleRig(ModelDocument doc, JigglePhysics.JiggleRigBuilder.JiggleRig rig)
	{
		ModelSpringRig modelRig = new ModelSpringRig();
		modelRig.jiggleRig = rig;
		modelRig.jiggleSettingsData = (rig.jiggleSettings as JigglePhysics.JiggleSettings).GetData();
		Transform rigRootTransform = rig.GetRootTransform();
		if (rigRootTransform is null)
		{
			Debug.LogWarning("Jiggle rig has no root transform, aborting spring rig export.");
			return modelRig;
		}
		// Add all descendant transforms as joint nodes.
		Transform[] descendants = rigRootTransform.GetComponentsInChildren<Transform>();
		for (int i = 0; i < descendants.Length; i++)
		{
			int nodeIndex = doc.FindNodeIndexByName(descendants[i].name);
			if (nodeIndex >= 0)
			{
				modelRig.jointNodeIndices.Add(nodeIndex);
			}
		}
		// Convert Naelstrof's JiggleSettings to VRM spring bone parameters using lossy heuristics.
		// Imperfect heuristic suggested by ChatGPT: dragForce = clamp01(friction + k_air * airDrag).
		modelRig.dragForce = Mathf.Clamp01(modelRig.jiggleSettingsData.friction + 0.5f * modelRig.jiggleSettingsData.airDrag);
		modelRig.gravityPower = modelRig.jiggleSettingsData.gravityMultiplier;
		if (modelRig.gravityPower < 0.0f)
		{
			// Inverted / upward gravity.
			modelRig.gravityPower = -modelRig.gravityPower;
			modelRig.gravityY = 1.0f;
		}
		modelRig.stiffness = modelRig.jiggleSettingsData.angleElasticity;
		modelRig.stiffness *= 1.0f - 0.5f * modelRig.jiggleSettingsData.elasticitySoften;
		modelRig.dragForce *= 1.0f + 0.5f * modelRig.jiggleSettingsData.elasticitySoften;
		// Set the name based on the spring rig root transform node's standardized name.
		int springRootNodeIndex = doc.FindNodeIndexByName(rigRootTransform.name);
		if (springRootNodeIndex < 0)
		{
			Debug.LogWarning("Could not find node for spring rig root transform: " + rigRootTransform.name);
			return modelRig;
		}
		ModelNode springRootNode = doc.nodes[springRootNodeIndex];
		modelRig.name = doc.ReserveUniqueName(springRootNode.name + "SpringRig");
		return modelRig;
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{\"joints\":[");
		for (int i = 0; i < jointNodeIndices.Count; i++)
		{
			if (i > 0) json.Append(",");
			json.Append("{");
			if (dragForce != 0.5f)
			{
				json.Append("\"dragForce\":");
				json.Append(Flt(dragForce));
				json.Append(",");
			}
			if (gravityY != -1.0f)
			{
				json.Append("\"gravityDir\":[0.0,");
				json.Append(Flt(gravityY));
				json.Append(",0.0],");
			}
			if (gravityPower != 0.0f)
			{
				json.Append("\"gravityPower\":");
				json.Append(Flt(gravityPower));
				json.Append(",");
			}
			json.Append("\"node\":");
			json.Append(jointNodeIndices[i]);
			// Note: Don't bother with VRM hitRadius because Yinglet Creator uses a zero radius for all spring bones.
			if (stiffness != 1.0f)
			{
				json.Append(",\"stiffness\":");
				json.Append(Flt(stiffness));
			}
			json.Append("}");
		}
		json.Append("],\"name\":\"");
		json.Append(name);
		json.Append("\"}");
		return json.ToString();
	}

	public string ToVRM0xJSON()
	{
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{");
		if (dragForce != 0.5f)
		{
			json.Append("\"dragForce\":");
			json.Append(Flt(dragForce));
			json.Append(",");
		}
		if (gravityY != -1.0f)
		{
			json.Append("\"gravityDir\":{\"x\":0.0,\"y\":");
			json.Append(Flt(gravityY));
			json.Append(",\"z\":0.0},");
		}
		if (gravityPower != 0.0f)
		{
			json.Append("\"gravityPower\":");
			json.Append(Flt(gravityPower));
			json.Append(",");
		}
		json.Append("\"bones\":[");
		// This is "the node index of the root bone of the swaying object" so I guess just one bone?
		json.Append(jointNodeIndices[0]);
		json.Append("]");
		// Note: Don't bother with VRM hitRadius because Yinglet Creator uses a zero radius for all spring bones.
		if (stiffness != 1.0f)
		{
			// Note: This typo is expected... because the VRM 0.x specification itself has this typo.
			json.Append(",\"stiffiness\":");
			json.Append(Flt(stiffness));
		}
		json.Append(",\"comment\":\"");
		json.Append(name);
		json.Append("\"}");
		return json.ToString();
	}
}
