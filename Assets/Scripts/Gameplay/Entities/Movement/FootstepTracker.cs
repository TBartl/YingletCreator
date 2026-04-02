using System;
using UnityEngine;

/// <summary>
/// Reports when a footstep occurs
/// </summary>
public interface IFootstepTracker
{
	event Action<Vector3> OnFootstep;
}

public class FootstepTracker : MonoBehaviour, IFootstepTracker
{

	private IYingletAnimationBridge _animation;

	// Track which step we last played (0 or 1 for .25 and .75)
	private int _lastStepIndex = 0;

	public event Action<Vector3> OnFootstep = delegate { };

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_animation = this.GetCharacterRootComponent<IYingletAnimationBridge>();
	}

	void Update()
	{
		float? animTime = _animation.GetMovingAnimTime();
		if (animTime == null)
		{
			_lastStepIndex = 0;
			return;
		}

		// We want to play a step sound at .25 and .75, so we add .25 to the time and then multiply by 2 to get a number where each whole number is a step
		// Slightly adjust down from .25f to better line up with audio
		int stepIndex = Mathf.FloorToInt((animTime.Value) * 2);

		if (stepIndex != _lastStepIndex)
		{
			OnFootstep(this.transform.position);
			_lastStepIndex = stepIndex;
		}
	}
}
