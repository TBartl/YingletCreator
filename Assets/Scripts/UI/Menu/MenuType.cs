using UnityEngine;

/// <summary>
/// Mostly a marker interface for the currently open menu
/// Has a few additional properties that are mostly set to drive consistent behavior around things like positioning and pressing escape
/// Set in <see cref="IMenuManager"/>
/// </summary>
[CreateAssetMenu(fileName = "MenuType", menuName = "Scriptable Objects/GenericUI/MenuType")]
public class MenuType : ScriptableObject
{
	[SerializeField] bool _popOnEscape = true;
	public bool PopOnEscape => _popOnEscape;

	[SerializeField] bool _popOnBackdropClicked = true;
	public bool PopOnBackdropClicked => _popOnBackdropClicked;

	[SerializeField] bool _restrictGameInput = true;
	public bool RestrictGameInput => _restrictGameInput;

	[SerializeField] bool _settingsSwapMenu = false;
	/// <summary>
	/// If true, this menu will be considered adjacent to other settings-like menus
	/// And swapped instead of pushed when swapping between them (see <see cref="SwapToMenuOnClick"/>)
	/// </summary>
	public bool SettingsSwapMenu => _settingsSwapMenu;
}
