using Reactivity;
using System.Linq;
using System.Text;
using TMPro;

public class ReflectNetStatusText : ReactiveBehaviour
{
	private INetStateProvider _netStateProvider;
	private INetClientTracker _netClientTracker;
	private TMP_Text _text;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_netStateProvider = Singletons.GetSingleton<INetStateProvider>();
		_netClientTracker = Singletons.GetSingleton<INetClientTracker>();
		_text = this.GetComponent<TMP_Text>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var sb = new StringBuilder();
		sb.Append("Status: ");
		if (_netStateProvider.IsHost)
		{
			sb.Append($"Hosting lobby for {_netClientTracker.Data.ClientIDs.Count()} player(s)");
		}
		else if (_netStateProvider.IsAttemptingClient)
		{
			sb.Append("Attempting to connect to host...");
		}
		else if (_netStateProvider.IsConnectedClient)
		{
			sb.Append($"Connected to lobby with {_netClientTracker.Data.ClientIDs.Count()} player(s)");
		}
		else
		{
			sb.Append("Solo");
		}

		_text.text = sb.ToString();
	}
}
