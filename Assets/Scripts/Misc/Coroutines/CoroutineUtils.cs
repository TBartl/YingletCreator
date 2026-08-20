using System;
using System.Collections;
using UnityEngine;

public interface IEaseSettings
{
	AnimationCurve Curve { get; }
	float Duration { get; }
}

[System.Serializable]
public sealed class EaseSettings : IEaseSettings
{
	[SerializeField] AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
	[SerializeField] float _duration = 1;

	public AnimationCurve Curve => _curve;
	public float Duration => _duration;
}

public static class CoroutineUtils
{
	public static void StopAndStartCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine existingCoroutine, IEnumerator routine)
	{
		if (existingCoroutine != null)
		{
			monoBehaviour.StopCoroutine(existingCoroutine);
		}

		existingCoroutine = monoBehaviour.StartCoroutine(routine);
	}

	public static void StartCoroutineIfNotAlreadyRunning(this MonoBehaviour monoBehaviour, ref Coroutine existingCoroutine, IEnumerator routine)
	{
		if (existingCoroutine != null)
		{
			return;
		}
		existingCoroutine = monoBehaviour.StartCoroutine(routine);
	}

	public static void StopCoroutineIfRunning(this MonoBehaviour monoBehaviour, ref Coroutine existingCoroutine)
	{
		if (existingCoroutine != null)
		{
			monoBehaviour.StopCoroutine(existingCoroutine);
			existingCoroutine = null;
		}
	}

	public static void StartEaseCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine existingCoroutine, IEaseSettings settings, Action<float> apply, Action onComplete = null)
	{
		if (!monoBehaviour.isActiveAndEnabled || !monoBehaviour.gameObject.activeInHierarchy)
		{
			apply(1);
			if (onComplete != null) onComplete();
			return;
		}
		StopAndStartCoroutine(monoBehaviour, ref existingCoroutine, Ease(settings, apply, onComplete));
	}

	static IEnumerator Ease(IEaseSettings settings, Action<float> apply, Action onComplete)
	{
		for (float t = 0; t < settings.Duration; t += Time.deltaTime)
		{
			float p = t / settings.Duration;
			apply(settings.Curve.Evaluate(p));
			yield return null;
		}
		apply(settings.Curve.Evaluate(1));
		if (onComplete != null) onComplete();
	}
}
