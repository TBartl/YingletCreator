using Reactivity;
using UnityEngine;

namespace Networking
{
	public interface INetIdentity
	{
		ulong NetId { get; }
		IReadOnlyObservable<ulong> NetIdObservable { get; }
		public GameObject gameObject { get; }
	}

	public interface IWriteableNetIdentity : INetIdentity
	{
		void ForceIdentity(ulong networkId);
	}

	[DefaultExecutionOrder(-5000)] // Very early to supercede anything using this. Can't use IInitializable because it's lazy
	public class NetIdentity : MonoBehaviour, IWriteableNetIdentity, IInitializable
	{
		Observable<ulong> _networkId = new Observable<ulong>();
		private INetIdentityProvider _idProvider;

		public ulong NetId => _networkId.Val;
		public IReadOnlyObservable<ulong> NetIdObservable => _networkId;

		public void ForceIdentity(ulong networkId)
		{
			// The server generated this ID, so we should do the same and discard it
			_idProvider.ScrapNextIdIfClient();
			_idProvider.RegisterIdentity(this, networkId);

			_networkId.Val = networkId;
		}

		public void Initialize()
		{
			_idProvider = Singletons.GetSingleton<INetIdentityProvider>();
		}

		private void Awake()
		{
			this.InitializeIfNeeded();

			// Something may have already forced our ID
			if (_networkId.Val == 0)
			{
				var nextId = _idProvider.GetNextId();
				_idProvider.RegisterIdentity(this, nextId);
				_networkId.Val = nextId;
			}
		}
		private void OnDestroy()
		{
			if (_networkId.Val != 0)
			{
				_idProvider.UnregisterIdentity(_networkId.Val);
			}
		}
	}
}