using Reactivity;

public class ShowOnNoNetState : ReactiveBehaviour
{
	void Start()
	{
		var netState = Singletons.GetSingleton<INetStateReader>();
		AddReflector(() =>
		{
			this.gameObject.SetActive(!netState.IsInAnyState);
		});
	}
}
