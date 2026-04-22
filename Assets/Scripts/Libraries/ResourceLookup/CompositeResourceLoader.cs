using Character.Compositor;
using Character.Data;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Loads resources combining:
/// - Pre-existing resources defined in the resource folder
/// - WIP mods, exposed via the editor resource folder
/// - Mods manually installed in the Yinglet Creator folder
/// - Mods installed via the steam workshop
/// </summary>
/// 
public interface ICompositeResourceLoader
{
	T Load<T>(string uniqueAssetId) where T : UnityEngine.Object;

	IEnumerable<CharacterToggleId> LoadAllToggleIds();
	IEnumerable<PoseId> LoadAllPoseIds();
	IEnumerable<PortraitId> LoadAllPortraitIds();
	IEnumerable<MixTexture> LoadMixTextures();
	IEnumerable<ClassId> LoadClasses();
}

public class CompositeResourceLoader : MonoBehaviour, ICompositeResourceLoader
{
	// Not a fan of this kind of singleton pattern, but exposing the loading otherwise is a major pain in the ass
	// Since loading logic happens from so many different locations
	public static ICompositeResourceLoader Instance { get; private set; }

	private IResourceProvider _provider;
	private IEnumerable<PoseId> _cachedPoses;
	private IEnumerable<CharacterToggleId> _cachedToggles;
	private IEnumerable<PortraitId> _cachedPortraits;
	private IEnumerable<MixTexture> _cachedTextures;
	private IEnumerable<ClassId> _cachedClasses;

	private void Awake()
	{
		Instance = this;

		_provider = GetProvider();
		_provider.Setup();

		_cachedPoses = _provider.LoadAll<PoseId>();
		_cachedToggles = _provider.LoadAll<CharacterToggleId>();
		_cachedPortraits = _provider.LoadAll<PortraitId>();
		_cachedTextures = _provider.LoadAll<MixTexture>();
		_cachedClasses = _provider.LoadAll<ClassId>();
	}

	private void OnDestroy()
	{
		if (System.Object.ReferenceEquals(Instance, this))
		{
			Instance = null;
		}
	}

	IResourceProvider GetProvider()
	{
		var loadMethod = Singletons.GetSingleton<IResourceLoadMethodProvider>().LoadMethod;
		return loadMethod switch
		{
			CompositeResourceLoadMethod.EditorAssetLookup => this.GetComponent<ResourceProvider_EditorAssetLookup>(),
			CompositeResourceLoadMethod.SerializedTableLookup => this.GetComponent<ResourceProvider_TableLookup>(),
			_ => throw new ArgumentException($"No composite resource loader for {loadMethod}")
		};
	}

	public T Load<T>(string uniqueAssetId) where T : UnityEngine.Object
	{
		return _provider.Load<T>(uniqueAssetId);
	}

	public IEnumerable<PoseId> LoadAllPoseIds()
	{
		return _cachedPoses;
	}
	public IEnumerable<CharacterToggleId> LoadAllToggleIds()
	{
		return _cachedToggles;
	}
	public IEnumerable<PortraitId> LoadAllPortraitIds()
	{
		return _cachedPortraits;
	}

	public IEnumerable<MixTexture> LoadMixTextures()
	{
		return _cachedTextures;
	}

	public IEnumerable<ClassId> LoadClasses()
	{
		return _cachedClasses;
	}
}

internal interface IResourceProvider
{
	void Setup();
	T Load<T>(string guid) where T : UnityEngine.Object;
	IEnumerable<T> LoadAll<T>() where T : UnityEngine.Object;
}