using Reactivity;

public class ShowOnHasLobby : ReactiveBehaviour
{
	void Start()
	{
		var netState = Singletons.GetSingleton<INetStateReader>();
		AddReflector(() =>
		{
			this.gameObject.SetActive(netState.CurrentLobby != null);
		});
	}
}
