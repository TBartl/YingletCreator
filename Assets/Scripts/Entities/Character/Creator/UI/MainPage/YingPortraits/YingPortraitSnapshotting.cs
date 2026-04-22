using Character.Creator;
using UnityEngine;
using UnityEngine.UI;

public class YingPortraitSnapshotting : MonoBehaviour
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
		_rt = _snapshotManager.GetRenderTexture(_reference);
		_image.texture = _rt.RenderTexture;
	}

	private void OnDestroy()
	{
		_image.texture = null;
		if (_rt != null)
		{
			_rt.Dispose();
		}
	}
}
