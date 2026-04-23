using Reactivity;

public class ShowIfCanStartExpedition : ReactiveBehaviour
{
	private IExpeditionManager _expeditionManager;
	private INetStateReader _netState;
	private IExpeditionPlanningManager _planning;

	void Start()
	{
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();
		_netState = Singletons.GetSingleton<INetStateReader>();
		_planning = Singletons.GetSingleton<IExpeditionPlanningManager>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		this.gameObject.SetActive(CanShow());
		bool CanShow()
		{
			if (_expeditionManager.State.Val == ExpeditionState.Running) return false;
			if (_planning.CurrentParty.Count == 0) return false;
			if (_netState.IsConnectedClient || _netState.IsAttemptingClient) return false;
			return true;
		}
	}
}
