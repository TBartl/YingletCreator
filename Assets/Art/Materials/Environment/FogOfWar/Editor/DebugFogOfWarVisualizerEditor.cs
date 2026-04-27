using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugFogOfWarVisualizer))]
public class DebugFogOfWarVisualizerEditor : Editor
{
	private Vector2Int roomPos;

	public override void OnInspectorGUI()
	{
		// Draw all the normal serialized fields
		DrawDefaultInspector();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

		// Vector2Int field
		roomPos = EditorGUILayout.Vector2IntField("RoomPos", roomPos);

		// Button
		if (GUILayout.Button("CarveRoom"))
		{
			DebugFogOfWarVisualizer visualizer = (DebugFogOfWarVisualizer)target;

			visualizer.CarveRoom(roomPos);
		}
	}
}