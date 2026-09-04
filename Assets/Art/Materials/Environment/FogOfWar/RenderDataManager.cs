using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface IRenderDataManager
{
	UniversalRendererData RendererData { get; }

	FogOfWarRendererFeature FogOfWarRenderFeature { get; }

	Material SourceFogOfWarMaterial { get; }
}

public class RenderDataManager : MonoBehaviour, IRenderDataManager, IInitializable
{
	[SerializeField] Material _sourceFogOfWarMaterial;
	[SerializeField] UniversalRendererData _rendererData;
	private FogOfWarRendererFeature _rendererFeature;

	public UniversalRendererData RendererData => _rendererData;
	public FogOfWarRendererFeature FogOfWarRenderFeature => _rendererFeature;

	public Material SourceFogOfWarMaterial => _sourceFogOfWarMaterial;

	public void Initialize()
	{
		_rendererFeature = _rendererData.rendererFeatures.OfType<FogOfWarRendererFeature>().First();

	}
}
