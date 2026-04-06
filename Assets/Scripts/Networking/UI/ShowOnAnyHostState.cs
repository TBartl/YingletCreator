using Reactivity;

public class ShowOnAnyHostState : ReactiveBehaviour
{
	void Start()
	{
		var netState = Singletons.GetSingleton<INetStateReader>();
		AddReflector(() =>
		{
			this.gameObject.SetActive(netState.IsAttemptingHost || netState.IsConnectedHost);
		});
	}
}
