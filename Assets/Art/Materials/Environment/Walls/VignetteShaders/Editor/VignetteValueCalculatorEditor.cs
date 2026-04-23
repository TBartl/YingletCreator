using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VignetteValueCalculator))]
public class VignetteValueCalculatorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// Draw default inspector (your three serialized fields)
		DrawDefaultInspector();

		var obj = (VignetteValueCalculator)target;

		// Get serialized values
		float distanceVisible = obj.DistanceVisible;
		float distanceBlackStart = obj.DistanceBlackStart;
		float distanceBlackEnd = obj.DistanceBlackEnd;

		EditorGUILayout.Space(10);
		EditorGUILayout.LabelField("Computed Shader Globals", EditorStyles.boldLabel);

		float diff = distanceVisible - distanceBlackStart;

		if (Mathf.Approximately(diff, 0f))
		{
			EditorGUILayout.HelpBox("DistanceVisible and DistanceBlackStart are equal — division by zero.", MessageType.Warning);
		}
		else
		{
			float m = 1f / diff;
			float b = -distanceBlackStart / diff;

			EditorGUILayout.LabelField("_VignetteSlope", m.ToString("F6"));
			EditorGUILayout.LabelField("_VignetteOffset", b.ToString("F6"));
			EditorGUILayout.LabelField("_VignetteRepeatDistance", distanceBlackEnd.ToString("F6"));
		}

		serializedObject.ApplyModifiedProperties();
	}
}