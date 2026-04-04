using Reactivity;

public interface IPlayerIdentity
{
	public ulong ConnectionId { get; set; }
	public bool IsMine { get; }
}

/// <summary>
/// Attached to the root player prefab, this component is used to identify the player by connection ID
/// </summary>
public class PlayerIdentity : ReactiveBehaviour, IPlayerIdentity
{
	Observable<ulong> _connectionId = new Observable<ulong>(0);
	private INetClientTracker _clientTracker;
	Computed<bool> _isMine;
	public ulong ConnectionId
	{
		get => _connectionId.Val;
		set => _connectionId.Val = value;
	}

	public bool IsMine => _isMine.Val;

	private void Awake()
	{
		_clientTracker = Singletons.GetSingleton<INetClientTracker>();
		_isMine = CreateComputed(() => _connectionId.Val == _clientTracker.LocalClientID);
	}
}
