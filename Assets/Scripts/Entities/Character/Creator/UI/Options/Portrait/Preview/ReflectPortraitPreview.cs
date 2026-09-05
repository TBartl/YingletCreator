using Character.Creator;
using Reactivity;
using Snapshotter;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Reflects the portrait on the character creator portrait page
/// This is the image that appears on the right
/// Doesn't use the conventional cached mechanism
/// Instead, declares its own render texture
/// </summary>
public class ReflectPortraitPreview : ReactiveBehaviour
{
	private RawImage _image;
	private IYingSnapshotManager _snapshotManager;
	private ICustomizationSelectedDataRepository _dataRepo;
	private IViewingPortraitPageTracker _viewingPortraitTracker;
	private RenderTexture _rt;

	private void Start()
	{
		_image = this.GetComponent<RawImage>();
		_snapshotManager = Singletons.GetSingleton<IYingSnapshotManager>();
		_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		_viewingPortraitTracker = this.GetComponentInParent<IViewingPortraitPageTracker>();


		_rt = SnapshotterUtils.CreateRenderTexture(_snapshotManager.References);
		_image.texture = _rt;

		AddReflector(Reflect);
	}

	void Reflect()
	{
		if (!_viewingPortraitTracker.IsViewingPortraitPage) return;

		// Following are just for reactivity and are otherwise unused
		var customizationData = _dataRepo.CustomizationData;
		var a = customizationData.PortraitData.UseOverrideExpressions.Val;
		var b = customizationData.PortraitData.OverrideEyeExpression.Val;
		var c = customizationData.PortraitData.OverrideMouthExpression.Val;
		var d = customizationData.ToggleData.Toggles.GetEnumerator(); // portrait itself is stored under toggles

		var parameters = new SnapshotterParams(_snapshotManager.CameraPosition, customizationData);

		_rt = SnapshotterUtils.Snapshot(
				_snapshotManager.References,
				parameters,
				_rt);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_image)
		{
			_image.texture = null;
		}
		_rt = null;
	}
}
