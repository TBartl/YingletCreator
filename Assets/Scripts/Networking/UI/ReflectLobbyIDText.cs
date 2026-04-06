using Reactivity;
using System.Text;
using TMPro;

public class ReflectLobbyIDText : ReactiveBehaviour
{
	private INetLobbyManager _lobbyManager;
	private TMP_Text _text;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_lobbyManager = Singletons.GetSingleton<INetLobbyManager>();
		_text = this.GetComponent<TMP_Text>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var sb = new StringBuilder();
		sb.Append("Lobby ID: ");
		var lobby = _lobbyManager.CurrentLobby;
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
