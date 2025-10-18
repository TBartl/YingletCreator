using UnityEngine;

public class SteamworksIntegration : MonoBehaviour
{
	void Start()
	{
		try
		{
			Steamworks.SteamClient.Init(3954540);
		}
		catch (System.Exception e)
		{
			Debug.Log("Error initializing steamworks: " + e.Message);
		}
	}

	void Update()
	{
		Steamworks.SteamClient.RunCallbacks();
	}

	private void OnDestroy()
	{
		Steamworks.SteamClient.Shutdown();
	}
}
