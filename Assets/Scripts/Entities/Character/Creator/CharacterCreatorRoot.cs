using UnityEngine;

/// <summary>
/// Stub class to make it easier to look up components
/// </summary>
public class CharacterCreatorRoot : MonoBehaviour
{

}

public static class CharacterCreatorRootExtensionMethods
{
	/// <summary>
	/// Returns the first component under the parent composited yinglet root
	/// </summary>
	public static T GetCharacterCreatorComponent<T>(this MonoBehaviour mb)
	{
		var type = typeof(T);
		var root = mb.GetComponentInParent<CharacterCreatorRoot>();
		if (root == null)
		{
			Debug.LogWarning($"Failed to get singleton component of type {type}; could not find character creator root");
			return default(T);
		}

		var component = root.GetComponentInChildren<T>(true);
		if (component == null)
		{
			Debug.LogWarning($"Failed to get singleton component of type {type}; could not find a component");
			return default(T);
		}
		return component;
	}
}