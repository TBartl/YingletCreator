using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(VertexColorBakingRoot))]
public class VertexColorBakingRootEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var root = target as VertexColorBakingRoot;

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Clear"))
		{
			VertexColorBakingLogic.ClearVertexColorData(root.transform);
		}
		if (GUILayout.Button("Clear (all)"))
		{
			var allBakingRoots = FindObjectsByType<VertexColorBakingRoot>();
			foreach (var bakingRoot in allBakingRoots)
			{
				VertexColorBakingLogic.ClearVertexColorData(bakingRoot.transform);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Bake"))
		{
			VertexColorBakingLogic.BakeVertexColors(root.transform, root._settings);
		}
		if (GUILayout.Button("Bake (all)"))
		{
			var allBakingRoots = FindObjectsByType<VertexColorBakingRoot>();
			foreach (var bakingRoot in allBakingRoots)
			{
				VertexColorBakingLogic.BakeVertexColors(bakingRoot.transform, bakingRoot._settings);
			}
		}
		GUILayout.EndHorizontal();

		if (GUILayout.Button("[DEPRECATED] Revert child prefab mesh filters"))
		{
			VertexColorBakingLogic.RevertChildMeshFilters(root.transform);
		}
	}
}