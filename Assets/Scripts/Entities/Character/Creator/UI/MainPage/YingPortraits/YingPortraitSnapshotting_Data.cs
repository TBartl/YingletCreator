using Character.Creator;
using Reactivity;
using UnityEngine.UI;

/// <summary>
/// Automatically obtains the snapshot for cached data
/// This is most useful for characters without an active object in the scene
/// since it will always generate a full new body to snapshot
/// However, it only supports customization data - not things from statuses/items
/// </summary>
public class YingPortraitSnapshotting_Data : ReactiveBehaviour
{
	private RawImage _image;
	private ICachedYingletReference _reference;
	private IYingSnapshotManager _snapshotManager;
	private IYingSnapshotRenderTexture _rt;

	private void Awake()
	{
		_image = this.GetComponent<RawImage>();
		_reference = GetComponentInParent<ICachedYingletReference>();
		_snapshotManager = Singletons.GetSingleton<IYingSnapshotManager>();
	}

	private void Start()
	{
		AddReflector(Reflect);
	}

	void Reflect()
	{
		_rt?.Dispose(); // Dispose any previously obtained render textures

		var cachedData = _reference.CachedData; // Get the cached data. This will likely be Observable
		_rt = _snapshotManager.GetDataRenderTexture(cachedData);
		if (_rt == null) return; // The cached data we provided may have been null
		_image.texture = _rt.RenderTexture;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		_image.texture = null;
		_rt?.Dispose();
	}
}
