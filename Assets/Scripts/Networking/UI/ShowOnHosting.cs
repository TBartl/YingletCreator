using Reactivity;
using UnityEngine;

public class ShowOnHosting : ReactiveBehaviour
{
	[SerializeField] bool _reverse;

	void Start()
	{
		var netState = Singletons.GetSingleton<INetStateProvider>();
		AddReflector(() =>
		{
			bool isHost = netState.IsHost;
			this.gameObject.SetActive(_reverse ? !isHost : isHost);
		});
	}
}
