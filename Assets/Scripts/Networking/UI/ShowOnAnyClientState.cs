using Reactivity;

public class ShowOnAnyClientState : ReactiveBehaviour
{
	void Start()
	{
		var netState = Singletons.GetSingleton<INetStateReader>();
		AddReflector(() =>
		{
			this.gameObject.SetActive(netState.IsAttemptingClient || netState.IsConnectedClient);
		});
	}
}
