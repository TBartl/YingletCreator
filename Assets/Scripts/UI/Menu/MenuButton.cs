using UnityEngine;

public enum MenuButtonType
{
	Exit,
	Settings,
	About,
	Discord,
	Screensaver

}

/// <summary>
/// Effectively a marker intereface
/// </summary>
public interface IMenuButton
{
	MenuButtonType Type { get; }
}

public class MenuButton : MonoBehaviour, IMenuButton
{
	[SerializeField] MenuButtonType _type;

	public MenuButtonType Type => _type;
}
