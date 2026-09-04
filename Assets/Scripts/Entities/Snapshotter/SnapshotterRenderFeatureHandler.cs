using System;

namespace Snapshotter
{
	internal sealed class SnapshotterRenderFeatureHandler : IDisposable
	{
		private IRenderDataManager _renderDataManager;

		public SnapshotterRenderFeatureHandler()
		{
			_renderDataManager = Singletons.GetSingleton<IRenderDataManager>();
			_renderDataManager.FogOfWarRenderFeature.SetActive(false);
		}

		public void Dispose()
		{
			_renderDataManager.FogOfWarRenderFeature.SetActive(true);
		}
	}
}
