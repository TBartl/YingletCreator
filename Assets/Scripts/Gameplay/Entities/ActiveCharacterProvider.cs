using Reactivity;
using System.Linq;

public interface IActiveCharacterProvider
{
	IReadOnlyObservable<ICharacterRoot> ActiveExpeditionCharacter { get; }
	IReadOnlyObservable<ICharacterRoot> ActiveCharacter { get; }
}

public class ActiveCharacterProvider : ReactiveBehaviour, IActiveCharacterProvider
{
	private INetStateReader _netState;
	Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	private ILobbyCharacterManager _lobbyCharacterManager;
	private Computed<ICharacterRoot> _activeExpeditionCharacter;
	private Computed<ICharacterRoot> _activeCharacter;

	public IReadOnlyObservable<ICharacterRoot> ActiveExpeditionCharacter => _activeExpeditionCharacter;
	public IReadOnlyObservable<ICharacterRoot> ActiveCharacter => _activeCharacter;

	private void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_lobbyCharacterManager = Singletons.GetSingleton<ILobbyCharacterManager>();
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>();
		_activeExpeditionCharacter = CreateComputed(ComputeActiveExpeditionCharacter);
		_activeCharacter = CreateComputed(ComputeActiveCharacter);
	}

	private ICharacterRoot ComputeActiveExpeditionCharacter()
	{
		var expeditionCharacterManager = _expeditionCharacterManager.Val;
		if (expeditionCharacterManager != null)
		{
			if (expeditionCharacterManager.ActiveCharacter.Val != null)
			{
				return expeditionCharacterManager.ActiveCharacter.Val.Root;
			}
		}
		return null;
	}

	private ICharacterRoot ComputeActiveCharacter()
	{
		var activeExpeditionCharacter = _activeExpeditionCharacter.Val;
		if (activeExpeditionCharacter != null)
		{
			return activeExpeditionCharacter;
		}

		var myLobbyCharacter = _lobbyCharacterManager.Characters.FirstOrDefault(c => c.Identity.OwnerClientId == _netState.LocalClientID);
		if (myLobbyCharacter != null)
		{
			return myLobbyCharacter.GameObject.GetComponentSafe<ICharacterRoot>();
		}

		return null;
	}
}
