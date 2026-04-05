using UnityEngine;

public class SteamManager : MonoBehaviour
{
	public const int SteamAppId = 3954540;

	private void Awake()
	{
		if (!Steamworks.SteamClient.IsValid)
		{
			try
			{
				Steamworks.SteamClient.Init(SteamAppId);
			}
			catch (System.Exception e)
			{
				// Continue running even if we can't connect to steam
				Debug.LogWarning(e);
			}
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
