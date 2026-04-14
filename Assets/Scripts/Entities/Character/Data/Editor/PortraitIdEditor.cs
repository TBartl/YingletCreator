using Character.Data;
using UnityEditor;

[CustomEditor(typeof(PortraitId)), CanEditMultipleObjects]
public class PortraitIdEditor : Editor
{
	OrderableScriptableObjectGuiDisplayer<PortraitId> _orderDisplayer = new();

	private void OnEnable()
	{
		_orderDisplayer.LoadAll();
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if (targets.Length > 1)
		{
			return;
		}

		_orderDisplayer.Display();
	}
}
