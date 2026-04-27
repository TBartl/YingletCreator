using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FogOfWarRendererFeature : ScriptableRendererFeature
{
	[System.Serializable]
	public class FogOfWarRendererData
	{
		public Material Material;
	}

	public FogOfWarRendererData Data { get; } = new FogOfWarRendererData();

	private FogOfWarRenderPass _fogOfWarRenderPass;

	public override void Create()
	{
		_fogOfWarRenderPass = new FogOfWarRenderPass(Data);

		_fogOfWarRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer,
		ref RenderingData renderingData)
	{
		if (_fogOfWarRenderPass == null)
		{
			return;
		}
		if (renderingData.cameraData.cameraType == CameraType.Game)
		{
			renderer.EnqueuePass(_fogOfWarRenderPass);
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}