using Character.Compositor;
using UnityEditor;

[CustomEditor(typeof(MixTexture)), CanEditMultipleObjects]
public class MixTextureEditor : Editor
{
	OrderableGroupedScriptableObjectGuiDisplayer<MixTexture, MixTextureOrderGroup> _orderDisplayer = new();

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

	MixTextureOrderGroup GetGroup()
	{
		var toggleId = target as MixTexture;
		return toggleId.Order.Group;
	}
}
