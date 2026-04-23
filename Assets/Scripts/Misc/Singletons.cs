using System;
using System.Collections.Generic;
using UnityEngine;


public class Singletons : MonoBehaviour
{
	static Singletons _instance;
	Dictionary<Type, object> _cache = new();

	private void Awake()
	{
		if (_instance != null) return;
		_instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	public static T GetSingleton<T>()
	{
		var type = typeof(T);
		if (_instance == null)
		{
			Debug.LogWarning($"Failed to get singleton of type {type}; no singleton object");
			return default(T);
		}
		if (_instance._cache.TryGetValue(type, out object cachedSingleton))
		{
			return (T)cachedSingleton;
		}

		var singleton = _instance.GetComponentInChildrenSafe<T>();
		if (singleton == null)
		{
			Debug.LogWarning($"Failed to get singleton of type {type}; could not find under singleton object");
			return default(T);
		}
		_instance._cache[type] = singleton;
		return singleton;
	}
}
