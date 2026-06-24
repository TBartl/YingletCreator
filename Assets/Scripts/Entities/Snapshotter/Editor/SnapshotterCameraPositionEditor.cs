using UnityEditor;
using UnityEngine;

namespace Snapshotter
{
	[CustomEditor(typeof(SnapshotterCameraPosition))]
	public class SnapshotterCameraPositionEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Draw the default inspector
			DrawDefaultInspector();

			// Add a space and a button
			EditorGUILayout.Space();
			if (GUILayout.Button("Capture Editor Transform"))
			{
				SceneView sceneView = SceneView.lastActiveSceneView;
				SnapshotterCameraPosition snapshot = (SnapshotterCameraPosition)target;

				// Record undo for editor
				Undo.RecordObject(snapshot, "Update Camera Position");

				// Set the values
				snapshot.Position = sceneView.camera.transform.position;
				snapshot.Rotation = sceneView.camera.transform.rotation.eulerAngles;

				// Mark as dirty so Unity saves the changes
				EditorUtility.SetDirty(snapshot);

				Debug.Log("Camera position and rotation captured.");
			}

			if (GUILayout.Button("Move Editor Camera Here"))
			{
				SnapshotterCameraPosition snapshot = (SnapshotterCameraPosition)target;
				SceneView sceneView = SceneView.lastActiveSceneView;
				sceneView.LookAtDirect(snapshot.Position, Quaternion.Euler(snapshot.Rotation), 0);
				SceneView.lastActiveSceneView.Repaint();
			}
		}
	}
}