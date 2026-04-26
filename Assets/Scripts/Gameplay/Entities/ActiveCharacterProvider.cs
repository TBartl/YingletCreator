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
			if (expeditionCharacterManager.ActiveCharacter.Val != null)
			{
				return expeditionCharacterManager.ActiveCharacter.Val.GameObject;
			}
		}

		var myLobbyCharacter = _lobbyCharacterManager.Characters.FirstOrDefault(c => c.Identity.OwnerClientId == _netState.LocalClientID);
		if (myLobbyCharacter != null)
		{
			return myLobbyCharacter.GameObject;
		}

		return null;
	}
}
