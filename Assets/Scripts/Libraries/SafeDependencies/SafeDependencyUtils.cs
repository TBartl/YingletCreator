using System.Runtime.CompilerServices;
using UnityEngine;

public static class SafeDependencyUtils
{
	private static readonly ConditionalWeakTable<object, object> _initialized = new();

	static void InitializeIfNeeded(object obj)
	{
		if (obj is IInitializable initializable)
		{
			if (!_initialized.TryGetValue(obj, out _))
			{
				// We don't have great protection from Circular dependencies
				// Just mark it as initialized before we go any further so if there are any, it might work
				_initialized.Add(obj, null);

				initializable.Initialize();
			}
		}
	}

	public static void InitializeIfNeeded(this IInitializable initializable)
	{
		InitializeIfNeeded((object)initializable);
	}

	public static T GetComponentSafe<T>(this Component component)
	{
		var result = component.GetComponent<T>();
		if (result == null)
		{
			Debug.LogError($"Component of type {typeof(T).Name} not found on GameObject {component.gameObject.name}.");
		}
		InitializeIfNeeded(result);
		return result;
	}
	public static T GetComponentSafe<T>(this GameObject gameObject)
	{
		var result = gameObject.GetComponent<T>();
		if (result == null)
		{
			Debug.LogError($"Component of type {typeof(T).Name} not found on GameObject {gameObject.name}.");
		}
		InitializeIfNeeded(result);
		return result;
	}

	public static T GetComponentInChildrenSafe<T>(this Component component)
	{
		var result = component.GetComponentInChildren<T>();
		if (result == null)
		{
			Debug.LogError($"Component of type {typeof(T).Name} not found in children of GameObject {component.gameObject.name}.");
		}
		InitializeIfNeeded(result);
		return result;
	}
	public static T GetComponentInChildrenSafe<T>(this GameObject gameObject)
	{
		var result = gameObject.GetComponentInChildren<T>();
		if (result == null)
		{
			Debug.LogError($"Component of type {typeof(T).Name} not found in children of GameObject {gameObject.name}.");
		}
		InitializeIfNeeded(result);
		return result;
	}

	public static T GetComponentInParentSafe<T>(this Component component)
	{
		var result = component.GetComponentInParent<T>();
		if (result == null)
		{
			Debug.LogError($"Component of type {typeof(T).Name} not found in parents of GameObject {component.gameObject.name}.");
		}
		InitializeIfNeeded(result);
		return result;
	}
	public static T GetComponentInParentSafe<T>(this GameObject gameObject)
	{
		var result = gameObject.GetComponentInParent<T>();
		if (result == null)
		{
			Debug.LogError($"Component of type {typeof(T).Name} not found in parents of GameObject {gameObject.name}.");
		}
		InitializeIfNeeded(result);
		return result;
	}
}
