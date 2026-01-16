using System;
using UnityEngine;

public interface IScreenTransitionManager
{
	void TransitionToOpaque();
	event Action OnStartTransitionToOpaque;
	IEaseSettings EaseSettings { get; }
}

public class ScreenTransitionManager : MonoBehaviour, IScreenTransitionManager
{
	[SerializeField] EaseSettings _easeSettings;

	public IEaseSettings EaseSettings => _easeSettings;

	public event Action OnStartTransitionToOpaque;

	public void TransitionToOpaque()
	{
		OnStartTransitionToOpaque?.Invoke();
	}
}
