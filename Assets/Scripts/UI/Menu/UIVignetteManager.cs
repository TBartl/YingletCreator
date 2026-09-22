using System;
using UnityEngine;

public interface IUIVignetteManager
{
	void FlashVignette(Color color);
	event Action<Color> OnFlashVignette;
}

public class UIVignetteManager : MonoBehaviour, IUIVignetteManager
{
	public event Action<Color> OnFlashVignette;
	public void FlashVignette(Color color)
	{
		OnFlashVignette?.Invoke(color);
	}
}
