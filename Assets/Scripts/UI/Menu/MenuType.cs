using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public enum MenuCoverType
{
	None,
	LeftSide,
	Everything
}

/// <summary>
/// Mostly a marker interface for the currently open menu
/// Has a few additional properties that are mostly set to drive consistent behavior around things like positioning and pressing escape
/// Set in <see cref="IMenuManager"/>
/// </summary>
[CreateAssetMenu(fileName = "MenuType", menuName = "Scriptable Objects/GenericUI/MenuType")]
public class MenuType : ScriptableObject
{
	[FormerlySerializedAs("_type")]
	[SerializeField] MenuCoverType _coverType = MenuCoverType.Everything;

	public MenuCoverType CoverType => _coverType;
}
