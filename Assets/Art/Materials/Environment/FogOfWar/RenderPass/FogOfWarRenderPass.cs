using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using static FogOfWarRendererFeature;

internal class FogOfWarRenderPass : ScriptableRenderPass
{
	private const string k_BlurTextureName = "_TestTexture";
	private const string PASS_NAME = nameof(FogOfWarRenderPass);
	private FogOfWarRendererData _data;
	private TextureDesc blurTextureDescriptor;

	public FogOfWarRenderPass(FogOfWarRendererData settings)
	{
		_data = settings;
	}

	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
	{
		if (_data?.Material == null) return;

		UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
		UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();


		TextureHandle srcCamColor = resourceData.activeColorTexture;
		blurTextureDescriptor = resourceData.activeColorTexture.GetDescriptor(renderGraph);
		blurTextureDescriptor.name = k_BlurTextureName;
		blurTextureDescriptor.depthBufferBits = 0;
		var dst = renderGraph.CreateTexture(blurTextureDescriptor);

		RenderGraphUtils.BlitMaterialParameters blitParams = new(dst, srcCamColor, _data.Material, 0);


		renderGraph.AddBlitPass(blitParams, PASS_NAME);
	}
}
