using Reactivity;
using System.Text;
using TMPro;

public class ReflectLobbyIDText : ReactiveBehaviour
{
	private INetStateReader _netState;
	private TMP_Text _text;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_text = this.GetComponent<TMP_Text>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var sb = new StringBuilder();
		sb.Append("Lobby ID: ");
		var lobby = _netState.CurrentLobby;
		if (lobby == null)
		{
			sb.Append("None");
		}
		else
		{
			sb.Append(lobby.Value.Id.Value);
		}

		_text.text = sb.ToString();
	}
}
