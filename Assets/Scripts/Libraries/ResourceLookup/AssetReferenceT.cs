using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Based off the Unity Engine's addressables AssetReferenceT
/// I ultimately found that system far too heavy handed to use
/// </summary>
[Serializable]
public class AssetReferenceT<T> : AssetReference where T : UnityEngine.Object
{
	bool _cached = false; // Can't rely on nullability because the value might be null
	T _cachedVal;

	public AssetReferenceT(string guid) : base()
	{
		m_AssetGUID = guid;
	}

	public T LoadSync()
	{
#if UNITY_EDITOR
		_cached = false; // Always reload in editor since it can actually change
#endif

		if (!_cached)
		{
#if UNITY_EDITOR
			if (CompositeResourceLoader.Instance == null) // For editor scripts that try to read off this
			{
				return EditorAsset as T;
			}
#endif


			_cachedVal = CompositeResourceLoader.Instance.Load<T>(m_AssetGUID);
			_cached = true;
		}

		return _cachedVal;
	}

#if UNITY_EDITOR
	public override UnityEngine.Object EditorAsset
	{
		get
		{
			var path = AssetDatabase.GUIDToAssetPath(m_AssetGUID);
			var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
			return asset;
		}
	}

	public override Type GetAssetType()
	{
		return typeof(T);
	}
#endif
}

public class AssetReference
{
	// Using this naming convention for compatibility reasons after migrating off of Addressables
	[SerializeField] protected string m_AssetGUID;

#if UNITY_EDITOR
	public string AssetGUID => m_AssetGUID;

	public virtual UnityEngine.Object EditorAsset
	{
		get => null;
	}

	public virtual Type GetAssetType()
	{
		return typeof(UnityEngine.Object);
	}

	public static string GetGuidFor(UnityEngine.Object obj)
	{
		var path = AssetDatabase.GetAssetPath(obj);
		var guid = AssetDatabase.GUIDFromAssetPath(path);
		return guid.ToString();
	}
#endif
}