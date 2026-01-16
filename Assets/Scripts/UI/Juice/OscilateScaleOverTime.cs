using UnityEngine;

internal class OscillateScaleOverTime : MonoBehaviour
{
	public float _amplitude = 0.2f;

	public float _frequency = 2f;

	private Vector3 _initialScale;

	void Start()
	{
		_initialScale = this.transform.localScale;
	}

	void Update()
	{
		float scaleFactor = 1 + Mathf.Sin(Time.time * _frequency) * _amplitude;
		transform.localScale = _initialScale * scaleFactor;
	}
}
