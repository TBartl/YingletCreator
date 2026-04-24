
using UnityEngine;

namespace Networking
{

	public interface INetIdentityProvider
	{
		ulong GetNextId();
		void ScrapNextIdIfClient();
	}

	internal class NetIdentityProvider : MonoBehaviour, INetIdentityProvider, IInitializable
	{
		private ulong _nextId = 1;
		private INetStateReader _netState;

		public void Initialize()
		{
			_netState = Singletons.GetSingleton<INetStateReader>();
		}

		public ulong GetNextId()
		{
			return _nextId++;
		}


		public void ScrapNextIdIfClient()
		{
			if (_netState.IsConnectedHost) return; // Server itself shouldn't scrap it since it generated it
			_nextId++;
		}
	}
}
