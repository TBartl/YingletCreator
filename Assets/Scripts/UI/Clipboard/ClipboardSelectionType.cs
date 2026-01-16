using UnityEngine;

namespace Character.Creator.UI
{

	/// <summary>
	/// Marker interface for the type of clipboard item (page/bookmark), used in <see cref="ClipboardSelection"/> among other things
	/// </summary>
	[CreateAssetMenu(fileName = "ClipboardSelectionType", menuName = "Scriptable Objects/GenericUI/ClipboardSelectionType")]
	public class ClipboardSelectionType : ScriptableObject { }
}
