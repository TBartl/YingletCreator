using System.Collections;
using UnityEngine;

public interface ICameraShaker
{
	void Shake();
}

public class CameraShaker : MonoBehaviour, ICameraShaker
{
	[SerializeField] float TOTAL_SHAKE_TIME;
	[SerializeField] float TIME_BETWEEN_SHAKES;
	[SerializeField] float MAX_RANGE;

	private Camera _mainCamera;
	private Coroutine _coroutine;

	void Start()
	{
		_mainCamera = Camera.main;
	}

	public void Shake()
	{
		this.StopAndStartCoroutine(ref _coroutine, ShakeCoroutine());
	}

	private IEnumerator ShakeCoroutine()
	{
		Vector3 fromDir = Vector3.zero;
		float startTime = Time.time;
		while (Time.time < startTime + TOTAL_SHAKE_TIME)
		{
			Vector3 toDir = Random.insideUnitSphere;

			for (float t = 0; t < TIME_BETWEEN_SHAKES; t += Time.deltaTime)
			{
				float p = t / TIME_BETWEEN_SHAKES;
				Vector3 dir = Vector3.Lerp(fromDir, toDir, p);
				float totalP = (Time.time - startTime) / TOTAL_SHAKE_TIME;
				_mainCamera.transform.localPosition = dir * MAX_RANGE * (1 - totalP);
				yield return null;
			}
			fromDir = toDir;
		}
		ResetAll();
	}

	void ResetAll()
	{
		_mainCamera.transform.localPosition = Vector3.zero;
	}
}
