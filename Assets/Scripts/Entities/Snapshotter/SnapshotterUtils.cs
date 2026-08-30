using UnityEngine;

namespace Snapshotter
{
	public static class SnapshotterUtils
	{
		public static RenderTexture Snapshot(ISnapshotterReferences references, SnapshotterParams sParams, RenderTexture renderTexture = null)
		{
			//var stopWatch = new Stopwatch();
			//stopWatch.Start();

			using var prefabHandler = new SnapshotterCharacterObjectHandler(references, sParams);
			using var cameraHandler = new SnapshotterCameraHandler(references, sParams);
			cameraHandler.OffsetPosByScale(prefabHandler.GetYScale());

			if (renderTexture == null)
			{
				renderTexture = CreateRenderTexture(references);
			}
			cameraHandler.RenderTo(renderTexture);

			//UnityEngine.Debug.Log($"Snapshot took {stopWatch.ElapsedMilliseconds}ms");
			return renderTexture;
		}

		public static RenderTexture CreateRenderTexture(ISnapshotterReferences references)
		{
			var renderTexture = new RenderTexture(references.SizeInPixels, references.SizeInPixels, 24);
			renderTexture.Create();
			return renderTexture;
		}
	}
}