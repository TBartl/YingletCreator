
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{

	public interface INetIdentityProvider
	{
		ulong GetNextId();
		void ScrapNextIdIfClient();
		void ForceNextId(ulong id);
		void RegisterIdentity(INetIdentity identity, ulong id);
		void UnregisterIdentity(ulong id);

		INetIdentity GetById(ulong id);
	}

	internal class NetIdentityProvider : MonoBehaviour, INetIdentityProvider, IInitializable
	{
		private ulong _nextId = 1;
		private INetStateReader _netState;

		Dictionary<ulong, INetIdentity> _identities = new Dictionary<ulong, INetIdentity>();

		public void Initialize()
		{
			_netState = Singletons.GetSingleton<INetStateReader>();
		}

		public ulong GetNextId()
		{
			var nextId = _nextId++;
			return nextId;
		}


		public void ScrapNextIdIfClient()
		{
			if (_netState.IsConnectedHost) return; // Server itself shouldn't scrap it since it generated it
			_nextId++;
		}

		public void ForceNextId(ulong id)
		{
			_nextId = id;
		}

		public void RegisterIdentity(INetIdentity identity, ulong id)
		{
			_identities[id] = identity;
		}

		public void UnregisterIdentity(ulong id)
		{
			_identities.Remove(id);
		}

		public INetIdentity GetById(ulong id)
		{
			if (_identities.TryGetValue(id, out var identity))
			{
				return identity;
			}
			else
			{
				Debug.LogWarning($"No identity found for id {id}");
				return null;
			}
		}
	}
}
