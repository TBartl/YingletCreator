using Character.Data;
using UnityEditor;

[CustomEditor(typeof(PoseId)), CanEditMultipleObjects]
public class PoseIdEditor : Editor
{
	OrderableGroupedScriptableObjectGuiDisplayer<PoseId, PoseOrderGroup> _orderDisplayer = new();

	private void OnEnable()
	{
		_orderDisplayer.LoadAll(GetGroup());
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if (targets.Length > 1)
		{
			return;
		}

		_orderDisplayer.Display(GetGroup());
	}

	PoseOrderGroup GetGroup()
	{
		var toggleId = target as PoseId;
		return toggleId.Order.Group;
	}
}
