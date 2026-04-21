using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IClientNameLookup
{
	string GetNameForClient(ulong clientId);
}

public class ClientNameLookup : MonoBehaviour, IClientNameLookup
{
	const string Unknown = "Unknown";

	private INetStateReader _netState;

	private void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();

	}

	public string GetNameForClient(ulong clientId)
	{
		try
		{
			var currentLobby = _netState.CurrentLobby;
			if (currentLobby != null)
			{
				Dictionary<ulong, SteamId> clients = _netState.SteamTransport.ConnectedClients;
				IEnumerable<Friend> lobbyMembers = currentLobby.Value.Members;

				foreach (var member in lobbyMembers)
				{
					var lobbyMemberSteamId = member.Id;
					if (clients.TryGetValue(clientId, out var clientSteamId) && clientSteamId == lobbyMemberSteamId)
					{
						return member.Name;
					}
				}
				throw new Exception($"Client {clientId} is not in the lobby");
			}
			try
			{

				if (!_netState.IsInAnyState && clientId == 0)
				{
					return SteamClient.Name;
				}
			}
			catch (NullReferenceException)
			{
				throw new Exception("Could not look up local client name - Steam is likely not running");
			}
			throw new Exception($"Client {clientId} is not in the lobby and we are not in a state where we can assume it's the local client");
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"Error looking up name for client {clientId}: {ex}");
			return Unknown;
		}
	}
}
