using Reactivity;
using UnityEngine;

public class ClockHandRotation : ReactiveBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;
	float zRot = 0;
	private IGlobalRoundProvider _globalRoundProvider;
	private Coroutine _transitionCoroutine;

	void Start()
	{
		_globalRoundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		int round = _globalRoundProvider.RoundManager?.CurrentRound?.Val ?? 0;

		float from = zRot;
		float to = round * -30;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, Apply);

		void Apply(float p)
		{
			zRot = Mathf.LerpUnclamped(from, to, p);
			this.transform.localRotation = Quaternion.Euler(0, 0, zRot);
		}
	}
}
