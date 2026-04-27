using UnityEngine;

public class DebugFogOfWarVisualizer : MonoBehaviour
{
	[SerializeField] Vector2Int NumRooms;
	private IFogOfWarCarver _carver;
	private MeshRenderer _meshRenderer;
	private DoubleBufferedRenderTexture _renderTextures;

	public IFogOfWarCarver Carver => _carver;

	private void Start()
	{
		_carver = this.GetComponentInParentSafe<IFogOfWarCarver>();
		_meshRenderer = this.GetComponentSafe<MeshRenderer>();

		_renderTextures = _carver.GenerateFogOfWarTexture(NumRooms);
		_meshRenderer.material.SetTexture("_MainTex", _renderTextures.GetCurrent());
	}

	public void CarveRoom(Vector2Int roomPos)
	{
		if (_carver == null)
		{
			Debug.LogError("No FogOfWarCarver yet. Are you trying to run this outside of Play mode?");
			return;
		}
		if (_renderTextures == null)
		{
			Debug.LogError("No RenderTexture yet. Have you initialized the texture?");
			return;
		}

		_carver.CarveRoom(_renderTextures, roomPos);
		_meshRenderer.material.SetTexture("_MainTex", _renderTextures.GetCurrent());
	}

	private void OnDestroy()
	{
		_renderTextures?.Cleanup();
	}
}
