using Reactivity;
using UnityEngine.UI;

/// <summary>
/// Snapshots a given character that is active in the scene
/// Unlike the Data version of this,
/// we use the player in the scene
/// </summary>
public class YingPortraitSnapshotting_Character : ReactiveBehaviour
{
	private RawImage _image;
	private IPartyMemberHUDReference _reference;
	private IYingSnapshotManager _snapshotManager;
	private IYingSnapshotRenderTexture _rt;

	private void Awake()
	{
		_image = this.GetComponent<RawImage>();
		_reference = GetComponentInParent<IPartyMemberHUDReference>();
		_snapshotManager = Singletons.GetSingleton<IYingSnapshotManager>();
	}

	private void Start()
	{
		AddReflector(Reflect);
	}

	void Reflect()
	{
		_rt?.Dispose(); // Dispose any previously obtained render textures

		var characterObservable = _reference.CharacterObservable;
		_rt = _snapshotManager.GetCharacterRenderTexture(characterObservable.Val);
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
