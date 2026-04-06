using Reactivity;

public class ShowWhenClient : ReactiveBehaviour
{
	void Start()
	{
		var netState = Singletons.GetSingleton<INetStateProvider>();
		AddReflector(() => this.gameObject.SetActive(netState.IsAttemptingClient || netState.IsConnectedClient));
	}
}
