using Reactivity;

public class ShowOnConnected : ReactiveBehaviour
{
	void Start()
	{
		var netState = Singletons.GetSingleton<INetStateProvider>();
		AddReflector(() => this.gameObject.SetActive(netState.IsRunning));
	}
}
