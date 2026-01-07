using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelMesh : ModelItem
{
	/// <summary>
	/// Right-handed mesh surface data converted from Unity meshes.
	/// </summary>
	public class ModelMeshSurface
	{
		public List<Vector2> textureMap;
		public List<int> triangles;
		public List<Vector3> vertices;
		public List<BoneWeight1[]> boneWeightsPerVertex;
		public byte maxBoneWeightsPerVertex = 0;

		public int materialIndex = -1;
		public int textureMapValuesAccessorIndex = -1;
		public int textureMapSimplexesAccessorIndex = -1;
		public int trianglesAccessorIndex = -1;
		// glTF only.
		public int verticesAccessorIndex = -1;
		public int skinJoints0AccessorIndex = -1;
		public int skinJoints1AccessorIndex = -1;
		public int skinWeights0AccessorIndex = -1;
		public int skinWeights1AccessorIndex = -1;
	}
	public List<ModelMeshSurface> surfaces = new List<ModelMeshSurface>();
	// G3MF only.
	public int sharedVerticesAccessorIndex = -1;
	private int _skinGroupsAccessorIndex = -1;
	private int _skinVerticesAccessorIndex = -1;
	private int _skinWeightsAccessorIndex = -1;

	public static ModelMesh FromNonEyeSkinnedMeshRenderers(ModelDocument doc, SkinnedMeshRenderer[] skinnedMeshes, bool mergeSurfaces)
	{
		ModelMesh mesh = new ModelMesh();
		Dictionary<int, int> nodeIndexToJointOrdinal = doc.GetNodeIndexToJointMap();
		// Iterate the skinned meshes to bake them and extract data, either merging into one surface, or creating separate surfaces.
		for (int i = 0; i < skinnedMeshes.Length; i++)
		{
			SkinnedMeshRenderer skinnedMesh = skinnedMeshes[i];
			if (skinnedMesh.name == "Eye-Left" || skinnedMesh.name == "Eye-Right")
			{
				continue;
			}
			if (mergeSurfaces && mesh.surfaces.Count > 0)
			{
				// Merge into the existing surface.
				mesh.surfaces[0] = SurfaceFromSkinnedMeshRenderer(doc, skinnedMesh, mesh.surfaces[0], 0.0f, nodeIndexToJointOrdinal);
			}
			else
			{
				// Create a new surface.
				mesh.surfaces.Add(SurfaceFromSkinnedMeshRenderer(doc, skinnedMesh, null, 0.0f, nodeIndexToJointOrdinal));
			}
		}
		return mesh;
	}

	public static ModelMeshSurface SurfaceFromEyeMeshes(ModelDocument doc, SkinnedMeshRenderer[] eyeMeshes)
	{
		ModelMeshSurface surface = null;
		Dictionary<int, int> nodeIndexToJointOrdinal = doc.GetNodeIndexToJointMap();
		// First pass: Eye bases, do not grow, but reweight to the head bone only.
		for (int meshIndex = 0; meshIndex < eyeMeshes.Length; meshIndex++)
		{
			SkinnedMeshRenderer skinnedMesh = eyeMeshes[meshIndex];
			surface = SurfaceFromSkinnedMeshRenderer(doc, skinnedMesh, surface, 0.0f, nodeIndexToJointOrdinal);
		}
		BoneWeight1 headBoneWeight = new BoneWeight1();
		headBoneWeight.boneIndex = nodeIndexToJointOrdinal[doc.FindNodeIndexByName("Head")];
		headBoneWeight.weight = 1.0f;
		BoneWeight1[] headBoneWeightForVert = new BoneWeight1[] { headBoneWeight };
		List<BoneWeight1[]> headWeightsPerVertex = new List<BoneWeight1[]>(surface.vertices.Count);
		for (int vertIndex = 0; vertIndex < surface.vertices.Count; vertIndex++)
		{
			headWeightsPerVertex.Add(headBoneWeightForVert);
		}
		surface.boneWeightsPerVertex = headWeightsPerVertex;
		// Second pass: Pupils, grow slightly to bring them in front, keep weights the same.
		for (int meshIndex = 0; meshIndex < eyeMeshes.Length; meshIndex++)
		{
			SkinnedMeshRenderer skinnedMesh = eyeMeshes[meshIndex];
			surface = SurfaceFromSkinnedMeshRenderer(doc, skinnedMesh, surface, 0.001f, nodeIndexToJointOrdinal);
		}
		return surface;
	}

	private static ModelMeshSurface SurfaceFromSkinnedMeshRenderer(ModelDocument doc, SkinnedMeshRenderer skinnedMesh, ModelMeshSurface surface, float growAmount, Dictionary<int, int> nodeIndexToJointOrdinal)
	{

		Mesh bakedMesh = new Mesh();
		skinnedMesh.BakeMesh(bakedMesh);
		if (skinnedMesh.name == "Eye-Left" || skinnedMesh.name == "Eye-Right")
		{
			GrowAlongSharedNormals(bakedMesh, growAmount);
		}
		Vector3[] bakedMeshVertices = bakedMesh.vertices;
		// Add vertices, converting to right-handed by flipping X.
		for (int vertIndex = 0; vertIndex < bakedMeshVertices.Length; vertIndex++)
		{
			bakedMeshVertices[vertIndex].x = -bakedMeshVertices[vertIndex].x;
		}
		List<BoneWeight1[]> skinnedMeshBoneWeightsPerVertex = null;
		byte maxBoneWeightsPerVertex = 0;
		if (nodeIndexToJointOrdinal != null)
		{
			// Remap the skinned mesh bone indices to the exported mesh bone joint indices.
			BoneWeight1[] remappedWeights;
			{
				Transform[] boneTransforms = skinnedMesh.bones;
				Unity.Collections.NativeArray<BoneWeight1> skinnedMeshWeights = skinnedMesh.sharedMesh.GetAllBoneWeights();
				remappedWeights = new BoneWeight1[skinnedMeshWeights.Length];
				for (int skinnedWeightIndex = 0; skinnedWeightIndex < skinnedMeshWeights.Length; skinnedWeightIndex++)
				{
					BoneWeight1 boneWeight = skinnedMeshWeights[skinnedWeightIndex];
					int originalBoneIndex = boneWeight.boneIndex;
					Transform boneTransform = boneTransforms[originalBoneIndex];
					int nodeIndex = doc.FindNodeIndexByName(boneTransform.name);
					if (nodeIndex < 0)
					{
						Debug.LogError($"Bone {boneTransform.name} not found in model document nodes.");
					}
					boneWeight.boneIndex = nodeIndexToJointOrdinal[nodeIndex];
					remappedWeights[skinnedWeightIndex] = boneWeight;
				}
			}
			// Add bone weights.
			Unity.Collections.NativeArray<byte> skinnedMeshBonesPerVert = skinnedMesh.sharedMesh.GetBonesPerVertex();
			if (skinnedMeshBonesPerVert.Length != bakedMeshVertices.Length)
			{
				Debug.LogError("Mismatch between skinned mesh bones per vertex length and baked mesh vertex count.");
			}
			skinnedMeshBoneWeightsPerVertex = new List<BoneWeight1[]>();
			int weightIndex = 0;
			for (int vertIndex = 0; vertIndex < skinnedMeshBonesPerVert.Length; vertIndex++)
			{
				byte numBones = skinnedMeshBonesPerVert[vertIndex];
				if (numBones > maxBoneWeightsPerVertex)
				{
					maxBoneWeightsPerVertex = numBones;
				}
				BoneWeight1[] thisVertWeights = new BoneWeight1[numBones];
				for (int boneIndex = 0; boneIndex < numBones; boneIndex++)
				{
					thisVertWeights[boneIndex] = remappedWeights[weightIndex];
					weightIndex++;
				}
				// Sort weights with the most influence first.
				System.Array.Sort(thisVertWeights, (a, b) => b.weight.CompareTo(a.weight));
				skinnedMeshBoneWeightsPerVertex.Add(thisVertWeights);
			}
		}
		// Put the data into a surface.
		int surfaceVertexOffset = 0;
		if (surface != null)
		{
			// Add to the existing surface.
			surfaceVertexOffset = surface.vertices.Count;
			if (skinnedMeshBoneWeightsPerVertex != null)
			{
				surface.maxBoneWeightsPerVertex = System.Math.Max(surface.maxBoneWeightsPerVertex, maxBoneWeightsPerVertex);
				surface.boneWeightsPerVertex.AddRange(skinnedMeshBoneWeightsPerVertex);
			}
			// Note: Don't merge texture maps here, they will be merged during the material atlasing step.
			surface.vertices.AddRange(bakedMeshVertices);
		}
		else
		{
			// Create a new surface.
			surface = new ModelMeshSurface();
			if (skinnedMeshBoneWeightsPerVertex != null)
			{
				surface.maxBoneWeightsPerVertex = maxBoneWeightsPerVertex;
				surface.boneWeightsPerVertex = skinnedMeshBoneWeightsPerVertex;
			}
			surface.textureMap = new List<Vector2>(bakedMesh.uv);
			surface.triangles = new List<int>();
			surface.vertices = new List<Vector3>(bakedMeshVertices);
		}
		// Add triangles, reversing the winding order for right-handed.
		int[] meshTriangles = bakedMesh.triangles;
		surface.triangles.Capacity += meshTriangles.Length;
		for (int t = 0; t < meshTriangles.Length; t += 3)
		{
			surface.triangles.Add(meshTriangles[t + 2] + surfaceVertexOffset);
			surface.triangles.Add(meshTriangles[t + 1] + surfaceVertexOffset);
			surface.triangles.Add(meshTriangles[t] + surfaceVertexOffset);
		}
		return surface;
	}

	private static void GrowAlongSharedNormals(Mesh mesh, float growAmount)
	{
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Dictionary<Vector3, Vector3> sharedNormalByVertex = new Dictionary<Vector3, Vector3>();
		for (int i = 0; i < vertices.Length; i++)
		{
			if (sharedNormalByVertex.ContainsKey(vertices[i]))
			{
				sharedNormalByVertex[vertices[i]] += normals[i];
			}
			else
			{
				sharedNormalByVertex[vertices[i]] = normals[i];
			}
		}
		foreach (var key in sharedNormalByVertex.Keys.ToArray())
		{
			sharedNormalByVertex[key] = sharedNormalByVertex[key].normalized;
		}
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] += sharedNormalByVertex[vertices[i]] * growAmount;
		}
		mesh.vertices = vertices;
	}

	/// <summary>
	/// General optimization pass that applies for both glTF and G3MF.
	/// </summary>
	public void DeduplicateWithinEachSurface()
	{
		for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
		{
			ModelMeshSurface surface = surfaces[surfaceIndex];
			int[] uniqueMap = new int[surface.vertices.Count];
			List<Vector3> uniqueVertices = new List<Vector3>();
			List<Vector2> uniqueTextureMap = new List<Vector2>();
			List<BoneWeight1[]> uniqueBoneWeightsPerVertex = new List<BoneWeight1[]>();
			for (int vertexIndex = 0; vertexIndex < surface.vertices.Count; vertexIndex++)
			{
				Vector3 vert = surface.vertices[vertexIndex];
				Vector2 uv = surface.textureMap[vertexIndex];
				BoneWeight1[] weights = surface.boneWeightsPerVertex[vertexIndex];
				// Check if this vertex is identical to an existing unique vertex.
				int uniqueIndex = -1;
				for (int uvi = 0; uvi < uniqueVertices.Count; uvi++)
				{
					if (uniqueVertices[uvi] != vert) continue;
					if (uniqueTextureMap[uvi] != uv) continue;
					BoneWeight1[] existingWeights = uniqueBoneWeightsPerVertex[uvi];
					if (existingWeights.Length != weights.Length) continue;
					bool weightsDiffer = false;
					for (int w = 0; w < weights.Length; w++)
					{
						if (existingWeights[w].boneIndex != weights[w].boneIndex || existingWeights[w].weight != weights[w].weight)
						{
							weightsDiffer = true;
							break;
						}
					}
					if (weightsDiffer) continue;
					uniqueIndex = uvi;
					break;
				}
				if (uniqueIndex < 0)
				{
					// No identical vertex found, add a new unique vertex.
					uniqueIndex = uniqueVertices.Count;
					uniqueVertices.Add(vert);
					uniqueTextureMap.Add(uv);
					uniqueBoneWeightsPerVertex.Add(weights);
				}
				uniqueMap[vertexIndex] = uniqueIndex;
			}
			List<int> newTriangles = new List<int>(surface.triangles.Count);
			for (int triIndex = 0; triIndex < surface.triangles.Count; triIndex++)
			{
				int originalVertIndex = surface.triangles[triIndex];
				int deduplicatedVertIndex = uniqueMap[originalVertIndex];
				newTriangles.Add(deduplicatedVertIndex);
			}
			surface.vertices = uniqueVertices;
			surface.textureMap = uniqueTextureMap;
			surface.boneWeightsPerVertex = uniqueBoneWeightsPerVertex;
			surface.triangles = newTriangles;
		}
	}

	public void EncodeMeshDataIntoAccessors(ModelDocument doc, ModelBaseFormat format)
	{
		if (format == ModelBaseFormat.GLTF)
		{
			// glTF's encoding of mesh data is very similar to Unity, so it's not difficult to convert.
			// Vertices, triangles, and texture map.
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				ModelMeshSurface surface = surfaces[surfaceIndex];
				surface.verticesAccessorIndex = ModelAccessor.EncodeVector3s(doc, surface.vertices, ModelBufferView.ArrayBufferglTFTarget.ARRAY_BUFFER);
				doc.accessors[surface.verticesAccessorIndex].SetMaxAndMinForVector3s(surface.vertices);
				surface.trianglesAccessorIndex = ModelAccessor.EncodeUIntsFromInt32s(doc, surface.triangles, ModelBufferView.ArrayBufferglTFTarget.ELEMENT_ARRAY_BUFFER);
				if (surface.textureMap != null && surface.textureMap.Count > 0)
				{
					// Flip texture V coordinate to match glTF and G3MF convention.
					// This is done here so that it occurs after all other processing such as atlasing.
					for (int uvIndex = 0; uvIndex < surface.textureMap.Count; uvIndex++)
					{
						Vector2 uv = surface.textureMap[uvIndex];
						uv.y = 1.0f - uv.y;
						surface.textureMap[uvIndex] = uv;
					}
					surface.textureMapValuesAccessorIndex = ModelAccessor.EncodeVector2s(doc, surface.textureMap, ModelBufferView.ArrayBufferglTFTarget.ARRAY_BUFFER);
				}
			}
			// Skinning data.
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				ModelMeshSurface surface = surfaces[surfaceIndex];
				if (surface.maxBoneWeightsPerVertex > 0)
				{
					Vector4Int[] skinJoints0 = new Vector4Int[surface.vertices.Count];
					Vector4[] skinWeights0 = new Vector4[surface.vertices.Count];
					for (int vertIndex = 0; vertIndex < surface.vertices.Count; vertIndex++)
					{
						BoneWeight1[] thisVertWeights = surface.boneWeightsPerVertex[vertIndex];
						for (int boneWeightIndex = 0; boneWeightIndex < Mathf.Min(4, thisVertWeights.Length); boneWeightIndex++)
						{
							skinJoints0[vertIndex][boneWeightIndex] = thisVertWeights[boneWeightIndex].boneIndex;
							skinWeights0[vertIndex][boneWeightIndex] = thisVertWeights[boneWeightIndex].weight;
						}
					}
					surface.skinJoints0AccessorIndex = ModelAccessor.EncodeUIntsFromVector4Ints(doc, skinJoints0, ModelBufferView.ArrayBufferglTFTarget.ARRAY_BUFFER);
					surface.skinWeights0AccessorIndex = ModelAccessor.EncodeVector4s(doc, skinWeights0, ModelBufferView.ArrayBufferglTFTarget.ARRAY_BUFFER);
				}
				if (surface.maxBoneWeightsPerVertex > 4)
				{
					Vector4Int[] skinJoints1 = new Vector4Int[surface.vertices.Count];
					Vector4[] skinWeights1 = new Vector4[surface.vertices.Count];
					for (int vertIndex = 0; vertIndex < surface.vertices.Count; vertIndex++)
					{
						BoneWeight1[] thisVertWeights = surface.boneWeightsPerVertex[vertIndex];
						for (int boneWeightIndex = 4; boneWeightIndex < thisVertWeights.Length; boneWeightIndex++)
						{
							skinJoints1[vertIndex][boneWeightIndex - 4] = thisVertWeights[boneWeightIndex].boneIndex;
							skinWeights1[vertIndex][boneWeightIndex - 4] = thisVertWeights[boneWeightIndex].weight;
						}
					}
					surface.skinJoints1AccessorIndex = ModelAccessor.EncodeUIntsFromVector4Ints(doc, skinJoints1, ModelBufferView.ArrayBufferglTFTarget.ARRAY_BUFFER);
					surface.skinWeights1AccessorIndex = ModelAccessor.EncodeVector4s(doc, skinWeights1, ModelBufferView.ArrayBufferglTFTarget.ARRAY_BUFFER);
				}
			}
		}
		else if (format == ModelBaseFormat.G3MF)
		{
			// G3MF uses a more efficient encoding that deduplicates vertices across surfaces,
			// decouples texture map data, and has skinning data unified between surfaces.
			// Vertices.
			List<Vector3> sharedVertices = new List<Vector3>();
			List<int[]> surfaceToSharedVertexIndices = new List<int[]>(surfaces.Count); // Exact capacity.
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				ModelMeshSurface surface = surfaces[surfaceIndex];
				// Exact capacity.
				int[] surfaceToSharedVertexIndex = new int[surface.vertices.Count];
				// Map surface vertices to shared vertices, deduplicating as we go.
				for (int vertIndex = 0; vertIndex < surface.vertices.Count; vertIndex++)
				{
					Vector3 vert = surface.vertices[vertIndex];
					// Check if this vertex is identical to an existing shared vertex.
					int sharedIndex = -1;
					for (int svi = 0; svi < sharedVertices.Count; svi++)
					{
						if (sharedVertices[svi] == vert)
						{
							sharedIndex = svi;
							break;
						}
					}
					if (sharedIndex < 0)
					{
						// No identical vertex found, add a new shared vertex.
						sharedIndex = sharedVertices.Count;
						sharedVertices.Add(vert);
					}
					surfaceToSharedVertexIndex[vertIndex] = sharedIndex;
				}
				surfaceToSharedVertexIndices.Add(surfaceToSharedVertexIndex);
			}
			sharedVerticesAccessorIndex = ModelAccessor.EncodeVector3s(doc, sharedVertices);
			// Triangles.
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				ModelMeshSurface surface = surfaces[surfaceIndex];
				int[] surfaceToSharedVertexIndex = surfaceToSharedVertexIndices[surfaceIndex];
				List<int> remappedTriangles = new List<int>(surface.triangles.Count); // Exact capacity.
				for (int triIndex = 0; triIndex < surface.triangles.Count; triIndex++)
				{
					int originalVertIndex = surface.triangles[triIndex];
					int sharedVertIndex = surfaceToSharedVertexIndex[originalVertIndex];
					remappedTriangles.Add(sharedVertIndex);
				}
				surface.trianglesAccessorIndex = ModelAccessor.EncodeUIntsFromInt32s(doc, remappedTriangles);
				doc.accessors[surface.trianglesAccessorIndex].vectorSize = 3; // Triangles.
			}
			// Texture map.
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				ModelMeshSurface surface = surfaces[surfaceIndex];
				if (surface.textureMap == null || surface.textureMap.Count != surface.vertices.Count)
				{
					continue;
				}
				List<int> remappedTextureMapSimplexes = new List<int>(surface.triangles.Count); // Exact capacity.
				List<Vector2> remappedTextureMapValues = new List<Vector2>(surface.textureMap.Count); // Over-estimation.
				Dictionary<Vector2, int> textureMapValueToIndex = new Dictionary<Vector2, int>();
				for (int triIndex = 0; triIndex < surface.triangles.Count; triIndex++)
				{
					int originalVertIndex = surface.triangles[triIndex];
					Vector2 uv = surface.textureMap[originalVertIndex];
					// Check if this UV is already in the values list.
					int uvIndex;
					if (!textureMapValueToIndex.TryGetValue(uv, out uvIndex))
					{
						// No identical UV found, add a new value.
						uvIndex = remappedTextureMapValues.Count;
						remappedTextureMapValues.Add(uv);
						textureMapValueToIndex[uv] = uvIndex;
					}
					remappedTextureMapSimplexes.Add(uvIndex);
				}
				surface.textureMapSimplexesAccessorIndex = ModelAccessor.EncodeUIntsFromInt32s(doc, remappedTextureMapSimplexes);
				surface.textureMapValuesAccessorIndex = ModelAccessor.EncodeVector2s(doc, remappedTextureMapValues);
				doc.accessors[surface.textureMapSimplexesAccessorIndex].vectorSize = 3; // Triangles.
			}
			// Skinning data.
			List<int> skinGroups = new List<int>();
			List<int> skinVertices = new List<int>();
			List<float> skinWeights = new List<float>();
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				ModelMeshSurface surface = surfaces[surfaceIndex];
				int[] surfaceToSharedVertexIndex = surfaceToSharedVertexIndices[surfaceIndex];
				HashSet<int> alreadyProcessedSharedVertices = new HashSet<int>();
				for (int vertIndex = 0; vertIndex < surface.vertices.Count; vertIndex++)
				{
					int sharedVertIndex = surfaceToSharedVertexIndex[vertIndex];
					if (alreadyProcessedSharedVertices.Contains(sharedVertIndex))
					{
						continue;
					}
					alreadyProcessedSharedVertices.Add(sharedVertIndex);
					BoneWeight1[] thisVertWeights = surface.boneWeightsPerVertex[vertIndex];
					for (int boneWeightIndex = 0; thisVertWeights != null && boneWeightIndex < thisVertWeights.Length; boneWeightIndex++)
					{
						skinGroups.Add(thisVertWeights[boneWeightIndex].boneIndex);
						skinVertices.Add(sharedVertIndex);
						skinWeights.Add(thisVertWeights[boneWeightIndex].weight);
					}
				}
			}
			_skinGroupsAccessorIndex = ModelAccessor.EncodeUIntsFromInt32s(doc, skinGroups);
			_skinVerticesAccessorIndex = ModelAccessor.EncodeUIntsFromInt32s(doc, skinVertices);
			_skinWeightsAccessorIndex = ModelAccessor.EncodeFloats(doc, skinWeights);
		}
	}

	public override string ModelItemToJSON(ModelBaseFormat format)
	{
		System.Text.StringBuilder json = new System.Text.StringBuilder();
		json.Append("{");
		if (format == ModelBaseFormat.GLTF)
		{
			if (name != null && name.Length > 0)
			{
				json.Append("\"name\":\"");
				json.Append(name);
				json.Append("\",");
			}
			json.Append("\"primitives\":[");
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				if (surfaceIndex > 0) json.Append(",");
				ModelMeshSurface surface = surfaces[surfaceIndex];
				json.Append("{"); // Start this primitive
				json.Append("\"attributes\":{");
				if (surface.skinJoints0AccessorIndex >= 0)
				{
					json.AppendFormat("\"JOINTS_0\":{0},", surface.skinJoints0AccessorIndex);
				}
				if (surface.skinJoints1AccessorIndex >= 0)
				{
					json.AppendFormat("\"JOINTS_1\":{0},", surface.skinJoints1AccessorIndex);
				}
				json.Append("\"POSITION\":" + surface.verticesAccessorIndex);
				if (surface.textureMapValuesAccessorIndex >= 0)
				{
					json.Append(",\"TEXCOORD_0\":" + surface.textureMapValuesAccessorIndex);
				}
				if (surface.skinWeights0AccessorIndex >= 0)
				{
					json.AppendFormat(",\"WEIGHTS_0\":{0}", surface.skinWeights0AccessorIndex);
				}
				if (surface.skinWeights1AccessorIndex >= 0)
				{
					json.AppendFormat(",\"WEIGHTS_1\":{0}", surface.skinWeights1AccessorIndex);
				}
				json.Append("}"); // End attributes
				if (surface.materialIndex >= 0)
				{
					json.AppendFormat(",\"material\":{0}", surface.materialIndex);
				}
				json.AppendFormat(",\"mode\":4,\"indices\":{0}", surface.trianglesAccessorIndex);
				json.Append("}"); // End this primitive
			}
			json.Append("]"); // End primitives array
		}
		else
		{
			if (name != null && name.Length > 0)
			{
				json.Append("\"name\":\"");
				json.Append(name);
				json.Append("\",");
			}
			if (_skinGroupsAccessorIndex >= 0 && _skinVerticesAccessorIndex >= 0 && _skinWeightsAccessorIndex >= 0)
			{
				json.Append("\"skin\":{");
				json.Append("\"groups\":" + _skinGroupsAccessorIndex);
				json.Append(",\"vertices\":" + _skinVerticesAccessorIndex);
				json.Append(",\"weights\":" + _skinWeightsAccessorIndex);
				json.Append("},");
			}
			json.Append("\"surfaces\":[");
			for (int surfaceIndex = 0; surfaceIndex < surfaces.Count; surfaceIndex++)
			{
				if (surfaceIndex > 0) json.Append(",");
				ModelMeshSurface surface = surfaces[surfaceIndex];
				json.Append("{"); // Start this surface
				if (surface.materialIndex >= 0)
				{
					json.AppendFormat("\"material\":{0},", surface.materialIndex);
				}
				json.Append("\"simplexes\":" + surface.trianglesAccessorIndex);
				if (surface.textureMapValuesAccessorIndex >= 0)
				{
					json.Append(",\"textureMap\":{");
					json.Append("\"simplexes\":" + surface.textureMapSimplexesAccessorIndex);
					json.Append(",\"values\":" + surface.textureMapValuesAccessorIndex);
					json.Append("}"); // End texture map
				}
				json.Append("}"); // End this surface
			}
			json.Append("]"); // End surfaces array
			json.Append(",\"vertices\":" + sharedVerticesAccessorIndex);
		}
		json.Append("}");
		return json.ToString();
	}
}
