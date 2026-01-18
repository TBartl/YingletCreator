using UnityEngine;


[System.Serializable]
public enum MenuTypeType
{
	None,
	LeftSide,
	CenterScreen
}

/// <summary>
/// Marker interface for the type of menu, used in <see cref="IMenuManager"/>
/// </summary>
[CreateAssetMenu(fileName = "MenuType", menuName = "Scriptable Objects/GenericUI/MenuType")]
public class MenuType : ScriptableObject
{
	[SerializeField] MenuTypeType _type = MenuTypeType.CenterScreen;

	public MenuTypeType Type => _type;
}
