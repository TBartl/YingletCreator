using UnityEngine;

internal class ServerSendCustomizationDataOnClientConnect : MonoBehaviour
{
	private ICharacterSpawner _characterSpawner;
	private INetEventBus _eventBus;
	private INetClientTracker _clientTracker;

	private void Start()
	{
		_clientTracker = Singletons.GetSingleton<INetClientTracker>();
		_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();

		_clientTracker.OnClientConnectedToUs += SendCustomizationDataToClient;
	}

	private void OnDestroy()
	{
		_clientTracker.OnClientConnectedToUs -= SendCustomizationDataToClient;
	}

	private void SendCustomizationDataToClient(ulong connectedClient)
	{
		foreach (var kvp in _characterSpawner.Characters)
		{
			var clientId = kvp.Key;
			if (clientId == connectedClient) continue; // Don't send the data to the client that just connected, they will already have it

			var character = kvp.Value;
			var characterNetworkData = character.GetComponentInChildren<INetworkCustomizationData>();
			var message = characterNetworkData.CreateMessage();
			_eventBus.SendToOne(message, connectedClient);
		}
	}
}
