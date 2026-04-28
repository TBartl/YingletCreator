using UnityEngine;

/// <summary>
/// Stub class to make it easier to look up components
/// </summary>
public sealed class CharacterRoot : MonoBehaviour
{
}

public static class CharacterRootExtensionMethods
{
	/// <summary>
	/// Returns the first component under the parent composited yinglet root
	/// </summary>
	public static T GetCharacterRootComponent<T>(this MonoBehaviour mb)
	{
		var type = typeof(T);
		var root = mb.GetComponentInParentSafe<CharacterRoot>();
		if (root == null)
		{
			Debug.LogWarning($"Failed to get component of type {type}; could not find character root");
			return default(T);
		}

		var component = root.GetComponentInChildrenSafe<T>();
		if (component == null)
		{
			Debug.LogWarning($"Failed to get component of type {type}; could not find a component");
			return default(T);
		}
		return component;
	}
}