using Character.Creator;
using Character.Creator.UI;
using UnityEditor;
using UnityEngine;

namespace Character.Compositor
{
	[CustomEditor(typeof(ColorGroup))]
	public class ColorGroupEditor : Editor
	{
		static Color _colorPickerColor = Color.white;

		public override void OnInspectorGUI()
		{

			ColorGroup myScript = (ColorGroup)target;

			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			// Draw default inspector UI
			DrawDefaultInspector();

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				UpdateGraphics();
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Base to color picker"))
			{
				_colorPickerColor = myScript.DefaultColors.Base.GetColor();
			}
			if (GUILayout.Button("Shade to color picker"))
			{
				_colorPickerColor = myScript.DefaultColors.Shade.GetColor();
			}
			GUILayout.EndHorizontal();
			_colorPickerColor = EditorGUILayout.ColorField("Color Picker", _colorPickerColor);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Color picker to base"))
			{
				Undo.RecordObject(myScript, "Changed Selected Color");
				((SerializableColorizeValuesPart)myScript.DefaultColors.Base).Set(_colorPickerColor);
				EditorUtility.SetDirty(myScript);
				UpdateGraphics();
			}
			if (GUILayout.Button("Color picker to shade"))
			{
				Undo.RecordObject(myScript, "Changed Selected Color");
				((SerializableColorizeValuesPart)myScript.DefaultColors.Shade).Set(_colorPickerColor);
				EditorUtility.SetDirty(myScript);
				UpdateGraphics();
			}
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Character creator color to base+shade"))
			{
				Undo.RecordObject(myScript, "Changed Selected Color");
				var activeSelection = FindAnyObjectByType<ColorActiveSelection>();
				var dataRepo = FindAnyObjectByType<CustomizationSelectedDataRepository>();
				var colorizeValues = dataRepo.GetColorizeValues(activeSelection.FirstSelected);
				((SerializableColorizeValuesPart)myScript.DefaultColors.Base).Set(colorizeValues.Base);
				((SerializableColorizeValuesPart)myScript.DefaultColors.Shade).Set(colorizeValues.Shade);
				EditorUtility.SetDirty(myScript);
				UpdateGraphics();
			}
		}

		static void UpdateGraphics()
		{

			//var composited = GameObject.FindObjectsByType<CompositedYinglet>(FindObjectsSortMode.None);
			//foreach (var c in composited)
			//{
			//	c.UpdateColorGroup();
			//}
		}
	}
}