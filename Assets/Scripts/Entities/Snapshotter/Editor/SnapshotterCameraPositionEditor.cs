using System.Linq;
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
			if (GUILayout.Button("Capture Camera Position and Rotation"))
			{
				Camera[] allCameras = Camera.allCameras;
				var sceneCamera = allCameras.Last();

				SnapshotterCameraPosition snapshot = (SnapshotterCameraPosition)target;

				// Record undo for editor
				Undo.RecordObject(snapshot, "Update Camera Position");

				// Set the values
				snapshot.Position = sceneCamera.transform.position;
				snapshot.Rotation = sceneCamera.transform.rotation.eulerAngles;

				// Mark as dirty so Unity saves the changes
				EditorUtility.SetDirty(snapshot);

				Debug.Log("Camera position and rotation captured.");
			}

			if (GUILayout.Button("Move Editor Camera Here"))
			{
				SnapshotterCameraPosition snapshot = (SnapshotterCameraPosition)target;
				SceneView sceneView = SceneView.lastActiveSceneView;
				//sceneView.pivot = snapshot.Position;
				//sceneView.rotation = Quaternion.Euler(snapshot.Rotation);
				sceneView.LookAtDirect(snapshot.Position, Quaternion.Euler(snapshot.Rotation));
				SceneView.lastActiveSceneView.Repaint();
			}
			if (GUILayout.Button("Move Game Camera Here"))
			{
				SnapshotterCameraPosition snapshot = (SnapshotterCameraPosition)target;
				var cam = FindAnyObjectByType<Camera>();
				cam.transform.position = snapshot.Position;
				cam.transform.rotation = Quaternion.Euler(snapshot.Rotation);
			}
		}
	}
}