using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a mechanism to access cached components
/// Normally I'd just expose this as a GameObject
/// But so many things want to access these that it's better to cache them a bit
/// </summary>
public interface ICharacterRoot
{
	GameObject gameObject { get; }
	Transform transform { get; }
	T GetComponentSafe<T>();
	T GetComponentInChildrenSafe<T>();

	/// <summary>
	/// Provided for debugging
	/// </summary>
	string name { get; }
}

public sealed class CharacterRoot : MonoBehaviour, ICharacterRoot
{
	Dictionary<Type, object> _cache = new();

	public T GetComponentSafe<T>()
	{
		var type = typeof(T);
		if (_cache.TryGetValue(type, out object cachedComponent))
		{
			return (T)cachedComponent;
		}
		var component = ((Component)this).GetComponentSafe<T>();
		if (component == null)
		{
			Debug.LogWarning($"Failed to get component of type {type}; could not find on character root");
			return default(T);
		}
		_cache[type] = component;
		return component;
	}

	public T GetComponentInChildrenSafe<T>()
	{
		var type = typeof(T);
		if (_cache.TryGetValue(type, out object cachedComponent))
		{
			return (T)cachedComponent;
		}
		var component = ((Component)this).GetComponentInChildrenSafe<T>();
		if (component == null)
		{
			Debug.LogWarning($"Failed to get component of type {type}; could not find under character root");
			return default(T);
		}
		_cache[type] = component;
		return component;
	}
}

public static class CharacterRootExtensionMethods
{
	/// <summary>
	/// Returns the first component under the parent composited yinglet root
	/// </summary>
	public static T GetCharacterRootComponent<T>(this MonoBehaviour mb)
	{
		var type = typeof(T);
		var root = mb.GetComponentInParentSafe<ICharacterRoot>(true);
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