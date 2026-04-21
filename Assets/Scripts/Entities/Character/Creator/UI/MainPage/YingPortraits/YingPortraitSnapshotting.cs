using Character.Creator.UI;
using UnityEngine;
using UnityEngine.UI;

public class YingPortraitSnapshotting : MonoBehaviour
{
	private RawImage _image;
	private IPortraitReference _reference;
	private IYingSnapshotManager _snapshotManager;
	private IYingSnapshotRenderTexture _rt;

	private void Awake()
	{
		_image = this.GetComponent<RawImage>();
		_reference = GetComponentInParent<IPortraitReference>();
		_snapshotManager = Singletons.GetSingleton<IYingSnapshotManager>();
	}

	private void Start()
	{
		_rt = _snapshotManager.GetRenderTexture(_reference.Reference);
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
