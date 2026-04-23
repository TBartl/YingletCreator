using System;
using UnityEngine;

public interface IScreenTransitionManager
{
	void TransitionToOpaque();
	void TransitionToTransparent();

	event Action OnStartTransitionToOpaque;
	event Action OnStartTransitionToTransparent;
	IEaseSettings EaseSettings { get; }
}

public class ScreenTransitionManager : MonoBehaviour, IScreenTransitionManager
{
	[SerializeField] EaseSettings _easeSettings;

	public IEaseSettings EaseSettings => _easeSettings;

	public event Action OnStartTransitionToOpaque;
	public event Action OnStartTransitionToTransparent;
	public void TransitionToOpaque()
	{
		OnStartTransitionToOpaque?.Invoke();
	}
	public void TransitionToTransparent()
	{
		OnStartTransitionToTransparent?.Invoke();
	}
}
