using UnityEngine;

// Hacky singleton solution to run coroutines without worrying about them being enabled/disabled

[DefaultExecutionOrder(-50000)]
public class CoroutineRunner : MonoBehaviour
{
	public static CoroutineRunner S { get; private set; }

	void Awake()
	{
		S = this;
	}
	private void OnDestroy()
	{
		if (S == this)
		{
			S = null;
		}
	}
}
