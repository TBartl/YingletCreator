using System;
using UnityEngine;

public static class RenderTextureUtils
{
	public static IDisposable TemporarilySetActive(RenderTexture renderTexture)
	{
		var previousActive = RenderTexture.active;
		RenderTexture.active = renderTexture;
		return new BasicActionDisposable(() => RenderTexture.active = previousActive);
	}
}
