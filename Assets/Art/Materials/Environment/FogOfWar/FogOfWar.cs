using Reactivity;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface IFogOfWar
{
	void RevealRoom(Vector2Int roomPosition);
	bool CheckRevealed(Vector2Int roomPosition);
}

public class FogOfWar : ReactiveBehaviour, IFogOfWar, IInitializable
{
	static readonly int MAIN_TEX_PROPERTY_ID = Shader.PropertyToID("_MainTex");
	static readonly int SCALE_PROPERTY_ID = Shader.PropertyToID("_Scale");
	static readonly int OFFSET_PROPERTY_ID = Shader.PropertyToID("_Offset");

	[SerializeField] Material _sourceMaterial;
	[SerializeField] UniversalRendererData _rendererData;

	private IRoomManager _roomManager;
	private IFogOfWarCarver _carver;
	private FogOfWarRendererFeature _rendererFeature;
	private Material _material; // TODO: Remove public 
	private DoubleBufferedRenderTexture _renderTextures;
	ObservableHashSet<Vector2Int> _revealedRooms = new ObservableHashSet<Vector2Int>();

	public void Initialize()
	{
		_roomManager = this.GetExpeditionComponent<IRoomManager>();
		_carver = this.GetComponentSafe<IFogOfWarCarver>();
		_rendererFeature = _rendererData.rendererFeatures.OfType<FogOfWarRendererFeature>().First();
		_material = new Material(_sourceMaterial);

		_rendererFeature.Data.Material = _material;

		var range = _roomManager.Range;
		Vector2Int totalSize = range.Max - range.Min + Vector2Int.one;
		_renderTextures = _carver.GenerateFogOfWarTexture(totalSize);

		// Fog of war is sampled by UV = WORLD_POS * SCALE + OFFSET
		var scale = (Vector2.one / totalSize) / RoomManager.ROOM_SIZE;
		var offset = (-range.Min + Vector2.one * 0.5f) / totalSize;
		_material.SetVector(SCALE_PROPERTY_ID, scale);
		_material.SetVector(OFFSET_PROPERTY_ID, offset);

		CarveRoom(new Vector2Int(0, 0));
	}

	private void Start()
	{
		this.InitializeIfNeeded();

		AddReflector(ReflectRenderTexture);
	}

	private void ReflectRenderTexture()
	{
		_material.SetTexture(MAIN_TEX_PROPERTY_ID, _renderTextures.GetCurrent());
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_rendererFeature != null)
		{
			_rendererFeature.Data.Material = null;
			Destroy(_material);
		}
		if (_renderTextures != null)
		{
			_renderTextures.Cleanup();
		}
	}

	void CarveRoom(Vector2Int position)
	{
		if (_revealedRooms.Contains(position))
		{
			return;
		}
		// Convert from room position to array position
		var carverPosition = position - _roomManager.Range.Min;
		_carver.CarveRoom(_renderTextures, carverPosition);
		_revealedRooms.Add(position);
	}

	public void RevealRoom(Vector2Int roomPosition)
	{
		CarveRoom(roomPosition);
	}

	public bool CheckRevealed(Vector2Int roomPosition)
	{
		return _revealedRooms.Contains(roomPosition);
	}
}
