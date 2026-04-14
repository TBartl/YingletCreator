using Character.Data;
using UnityEditor;

[CustomEditor(typeof(CharacterToggleId)), CanEditMultipleObjects]
public class CharacterToggleIdEditor : Editor
{
    OrderableGroupedScriptableObjectGuiDisplayer<CharacterToggleId, CharacterToggleOrderGroup> _orderDisplayer = new();

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

    CharacterToggleOrderGroup GetGroup()
    {
        var toggleId = target as CharacterToggleId;
        return toggleId.Order.Group;
    }
}
