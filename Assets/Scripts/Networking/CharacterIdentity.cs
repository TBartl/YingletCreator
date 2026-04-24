using Reactivity;

namespace Networking
{
	public interface ICharacterIdentity : INetIdentity
	{
		ulong OwnerClientId { get; }
		IReadOnlyObservable<ulong> OwnerClientIdObservable { get; }
		bool IsMine { get; }

		// Not sure this fits here, but will leave it for now
		bool IsActive { get; }
	}

	public interface IWriteableCharacterIdentity : ICharacterIdentity, IWriteableNetIdentity
	{
		void SetOwner(ulong ownerClientId);
	}

	/// <summary>
	/// Attached to the root player prefab, this component is used to identify the player by connection ID
	/// </summary>
	public class CharacterIdentity : ReactiveBehaviour, IWriteableCharacterIdentity, IInitializable
	{
		Observable<ulong> _ownerId = new Observable<ulong>(0);
		private INetClientTracker _clientTracker;
		private IActiveCharacterProvider _activeCharacterProvider;
		private IWriteableNetIdentity _netIdentity;
		Computed<bool> _isMine;
		Computed<bool> _isActive;
		public ulong OwnerClientId => _ownerId.Val;
		public IReadOnlyObservable<ulong> OwnerClientIdObservable => _ownerId;

		public bool IsMine => _isMine.Val;

		public bool IsActive => _isActive.Val;

		public ulong NetId => _netIdentity.NetId;

		public IReadOnlyObservable<ulong> NetIdObservable => _netIdentity.NetIdObservable;

		public void ForceIdentity(ulong networkId)
		{
			_netIdentity.ForceIdentity(networkId);
		}

		public void SetOwner(ulong ownerClientId)
		{
			_ownerId.Val = ownerClientId;
		}

		public void Initialize()
		{
			_clientTracker = Singletons.GetSingleton<INetClientTracker>();
			_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
			_netIdentity = this.GetComponentSafe<IWriteableNetIdentity>();
			_isMine = CreateComputed(() => _ownerId.Val == _clientTracker.LocalClientID);
			_isActive = CreateComputed(() => _activeCharacterProvider.ActiveCharacter.Val == this.gameObject);
		}
	}
}
