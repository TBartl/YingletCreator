using Unity.Netcode;

public static class NetworkManagerExtensionMethods
{
	public static bool IsPureClient(this NetworkManager networkManager)
	{
		return networkManager.IsClient && !networkManager.IsServer;
	}
}
