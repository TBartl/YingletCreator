using Reactivity.Implementation;
using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Render textures can't natively be used for read and write at the same time
/// The solution is to use two render textures and swap between them
/// Unity provides something for this in the form of CustomRenderTexture.doubleBuffered,
/// but it doesn't give me enough control:
/// I want to be able to clean up just the remaining render texture at the end
/// </summary>
public sealed class DoubleBufferedRenderTexture : IDisposable
{
	RenderTexture _upToDate;
	RenderTexture _backup;
	Notifier _notifier = new Notifier(); // Render textures don't play nicely with Observable. At least, my unity keeps crashing

	public DoubleBufferedRenderTexture(Vector2Int textureSize)
		: this(textureSize, _ => { }) { }
	public DoubleBufferedRenderTexture(Vector2Int textureSize, System.Action<RenderTexture> beforeCreate)
	{
		_upToDate = CreateRT();
		_backup = CreateRT();

		RenderTexture CreateRT()
		{
			var rt = new RenderTexture(textureSize.x, textureSize.y, 0);
			rt.wrapMode = TextureWrapMode.Clamp;
			beforeCreate(rt);
			rt.Create();
			ClearRt(rt, Color.clear);

			return rt;
		}
	}

	static void ClearRt(RenderTexture rt, Color clear)
	{
		var prev = RenderTexture.active;
		RenderTexture.active = rt;
		GL.Clear(true, true, clear);
		RenderTexture.active = prev;
	}

	public void Blit(Material mat)
	{
		Assert.IsNotNull(_backup);

		Graphics.Blit(_upToDate, _backup, mat);
		Swap();
	}

	public RenderTexture Finalize()
	{
		if (_backup != null)
		{
			_backup.Release();
			SmartDestroy(_backup);
			_backup = null;
		}

		return _upToDate;
	}

	public RenderTexture GetCurrent()
	{
		_notifier.Track();
		return _upToDate;
	}

	public void Dispose()
	{
		if (_upToDate != null)
		{
			_upToDate.Release();
			SmartDestroy(_upToDate);
			_upToDate = null;
		}
		if (_backup != null)
		{
			_backup.Release();
			SmartDestroy(_backup);
			_backup = null;
		}
	}

	static void SmartDestroy(UnityEngine.Object obj)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			GameObject.DestroyImmediate(obj);
		}
		else
#endif
		{
			GameObject.Destroy(obj);
		}
	}

	void Swap()
	{
		RenderTexture temp = _upToDate;
		_upToDate = _backup;
		_backup = temp;
		_notifier.Dirty();
	}
}
