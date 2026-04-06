using Reactivity;
using System.Linq;
using System.Text;
using TMPro;

public class ReflectNetStatusText : ReactiveBehaviour
{
	private INetStateReader _netState;
	private INetClientTracker _netClientTracker;
	private TMP_Text _text;

	Computed<int> _numClients;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_netClientTracker = Singletons.GetSingleton<INetClientTracker>();
		_text = this.GetComponent<TMP_Text>();

		_numClients = CreateComputed(() => _netClientTracker.Data.ClientIDs.Count());
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var sb = new StringBuilder();
		sb.Append("Status: ");
		if (_netState.IsAttemptingHost)
		{
			sb.Append("Creating lobby...");
		}
		else if (_netState.IsConnectedHost)
		{
			sb.Append($"Hosting lobby for {_numClients.Val} player(s)");
		}
		else if (_netState.IsAttemptingClient)
		{
			sb.Append("Attempting to connect to lobby...");
		}
		else if (_netState.IsConnectedClient)
		{
			sb.Append($"Connected to lobby with {_numClients.Val} player(s)");
		}
		else
		{
			sb.Append("Solo");
		}

		_text.text = sb.ToString();
	}
}
