using UnityEngine;

public class ScreenTransitionOnExpeditionStateChanged : MonoBehaviour
{
	private IExpeditionManager _expeditionManager;
	private IScreenTransitionManager _transitionManager;

	private void Start()
	{
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();
		_transitionManager = Singletons.GetSingleton<IScreenTransitionManager>();

		_expeditionManager.State.OnChanged += OnExpeditionStateChanged;
	}

	private void OnExpeditionStateChanged(ExpeditionState from, ExpeditionState to)
	{
		if (to == ExpeditionState.Starting)
		{
			_transitionManager.TransitionToOpaque();
		}
		else
		{
			_transitionManager.TransitionToTransparent();
		}
	}
}
