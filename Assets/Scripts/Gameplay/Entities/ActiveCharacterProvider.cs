using Reactivity;
using System.Linq;
using UnityEngine;

public interface IActiveCharacterProvider
{
	IReadOnlyObservable<GameObject> ActiveExpeditionCharacter { get; }
	IReadOnlyObservable<GameObject> ActiveCharacter { get; }
}

public class ActiveCharacterProvider : ReactiveBehaviour, IActiveCharacterProvider
{
	private INetStateReader _netState;
	Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	private ILobbyCharacterManager _lobbyCharacterManager;
	private Computed<GameObject> _activeExpeditionCharacter;
	private Computed<GameObject> _activeCharacter;

	public IReadOnlyObservable<GameObject> ActiveExpeditionCharacter => _activeExpeditionCharacter;
	public IReadOnlyObservable<GameObject> ActiveCharacter => _activeCharacter;

	private void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_lobbyCharacterManager = Singletons.GetSingleton<ILobbyCharacterManager>();
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>();
		_activeExpeditionCharacter = CreateComputed(ComputeActiveExpeditionCharacter);
		_activeCharacter = CreateComputed(ComputeActiveCharacter);
	}

	private GameObject ComputeActiveExpeditionCharacter()
	{
		var expeditionCharacterManager = _expeditionCharacterManager.Val;
		if (expeditionCharacterManager != null)
		{
			if (expeditionCharacterManager.ActiveCharacter.Val != null)
			{
				return expeditionCharacterManager.ActiveCharacter.Val.GameObject;
			}
		}
		return null;
	}

	private GameObject ComputeActiveCharacter()
	{
		var activeExpeditionCharacter = _activeExpeditionCharacter.Val;
		if (activeExpeditionCharacter != null)
		{
			return activeExpeditionCharacter;
		}

		var myLobbyCharacter = _lobbyCharacterManager.Characters.FirstOrDefault(c => c.Identity.OwnerClientId == _netState.LocalClientID);
		if (myLobbyCharacter != null)
		{
			return myLobbyCharacter.GameObject;
		}

		return null;
	}
}
