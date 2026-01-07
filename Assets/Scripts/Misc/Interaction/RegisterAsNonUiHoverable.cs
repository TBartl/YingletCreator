using UnityEngine;

/// <summary>
/// This is the hoverable, fullscreen area on the bottom layer of the canvas
/// Used to determine if the mouse is hovering over the UI or not.
/// </summary>
[RequireComponent(typeof(UiHoverable))]
public class RegisterAsNonUiHoverable : MonoBehaviour
{
	private void Awake()
	{
		var uiHoverManager = Singletons.GetSingleton<IWriteableUiHoverManager>();
		uiHoverManager.RegisterNonUiHoverable(this.GetComponent<IHoverable>());
	}
}
