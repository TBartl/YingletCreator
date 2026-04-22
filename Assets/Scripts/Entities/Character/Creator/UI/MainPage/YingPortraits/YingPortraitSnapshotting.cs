using Character.Creator;
using Reactivity;
using UnityEngine.UI;

public class YingPortraitSnapshotting : ReactiveBehaviour
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
		_rt = _snapshotManager.GetRenderTexture(cachedData);
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
