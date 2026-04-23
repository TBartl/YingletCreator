using Reactivity;
using System.Linq;
using UnityEngine;

public interface IActiveCharacterProvider
{
	IReadOnlyObservable<GameObject> ActiveCharacter { get; }
}

public class ActiveCharacterProvider : ReactiveBehaviour, IActiveCharacterProvider
{
	private INetStateReader _netState;
	Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	private ILobbyCharacterManager _lobbyCharacterManager;
	private Computed<GameObject> _activeCharacter;

	public IReadOnlyObservable<GameObject> ActiveCharacter => _activeCharacter;

	private void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_lobbyCharacterManager = Singletons.GetSingleton<ILobbyCharacterManager>();
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>(Singletons.GetSingleton<IExpeditionManager>());
		_activeCharacter = CreateComputed(ComputeActiveCharacter);
	}

	private GameObject ComputeActiveCharacter()
	{
		var expeditionCharacterManager = _expeditionCharacterManager.Val;
		if (expeditionCharacterManager != null)
		{
			var validCharacters = expeditionCharacterManager.Characters.Where(c => c.ClientId == _netState.LocalClientID);
			if (validCharacters.Any())
			{
				// TODO: Use _testOffset
				return validCharacters.ElementAt(_testOffset.Val % validCharacters.Count()).GameObject;
			}
		}

		foreach (var lobbyCharacter in _lobbyCharacterManager.LobbyCharacters)
		{
			if (lobbyCharacter.ClientId == _netState.LocalClientID)
			{
				return lobbyCharacter.GameObject;
			}
		}
		return null;
	}

	// Logic to be moved
	Observable<int> _testOffset = new Observable<int>(0);
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			_testOffset.Val += 1;
		}
	}
}
