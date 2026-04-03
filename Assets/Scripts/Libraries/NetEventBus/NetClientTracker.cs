using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public interface INetClientTracker
{
	ulong LocalClientID { get; }
	IEnumerable<ulong> ClientIDs { get; }
}

internal sealed class NetClientTracker : MonoBehaviour, INetClientTracker
{
	private NetworkManager _netManager;

	// Start with ID 0
	Observable<ulong> _localClientId = new Observable<ulong>(0);
	ObservableList<ulong> _clientIds = new ObservableList<ulong>(new ulong[] { 0 });

	public ulong LocalClientID => _localClientId.value;

	public IEnumerable<ulong> ClientIDs => _clientIds;

	private void Start()
	{
		_netManager = NetworkManager.Singleton;
	}
}
