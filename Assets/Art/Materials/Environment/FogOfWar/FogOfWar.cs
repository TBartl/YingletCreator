using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface IFogOfWar
{
	void RevealRoom(IRoom room);
}

public class FogOfWar : ReactiveBehaviour, IFogOfWar, IInitializable
{
	static readonly int MAIN_TEX_PROPERTY_ID = Shader.PropertyToID("_MainTex");
	static readonly int SCALE_PROPERTY_ID = Shader.PropertyToID("_Scale");
	static readonly int OFFSET_PROPERTY_ID = Shader.PropertyToID("_Offset");

	[SerializeField] Material _sourceMaterial;
	[SerializeField] Vector2Int _debugNumRooms;
	[SerializeField] UniversalRendererData _rendererData;

	private IFogOfWarCarver _carver;
	private FogOfWarRendererFeature _rendererFeature;
	private Material _material;
	private DoubleBufferedRenderTexture _renderTextures;
	HashSet<Vector2Int> _revealedRooms = new HashSet<Vector2Int>();

	public void Initialize()
	{
		_carver = this.GetComponentSafe<IFogOfWarCarver>();
		_rendererFeature = _rendererData.rendererFeatures.OfType<FogOfWarRendererFeature>().First();
		_material = new Material(_sourceMaterial);

		_rendererFeature.Data.Material = _material;

		_renderTextures = _carver.GenerateFogOfWarTexture(_debugNumRooms);

		const int roomSize = 6;
		_material.SetVector(SCALE_PROPERTY_ID, (Vector2.one / _debugNumRooms) / roomSize);
		_material.SetVector(OFFSET_PROPERTY_ID, new Vector2(0.5f, 0.5f));
	}

	private void Start()
	{
		this.InitializeIfNeeded();

		CarveRoom(new Vector2Int(2, 2));
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

	void CarveRoom(Vector2Int vector2Int)
	{
		if (_revealedRooms.Contains(vector2Int))
		{
			return;
		}
		_carver.CarveRoom(_renderTextures, vector2Int);
		_revealedRooms.Add(vector2Int);
	}

	public void RevealRoom(IRoom room)
	{
		CarveRoom(room.Position);
	}
}
