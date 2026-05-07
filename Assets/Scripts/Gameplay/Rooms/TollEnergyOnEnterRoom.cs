using System;
using UnityEngine;

public interface ITollEnergyOnEnterRoom
{
	int GetCostToEnterRoom(IRoom room);
	bool CanAffordEntry(int cost);
	event Action<int> OnEnergyTollApplied;
}

public class TollEnergyOnEnterRoom : MonoBehaviour, ITollEnergyOnEnterRoom
{
	public const int DiscoveryEnergyCost = 2;
	public const int ReEntryEnergyCost = 1;
	private ICommonGameplayAssets _assets;
	private ICharacterRoomDetector _characterRoomDetector;
	private IFogOfWar _fogOfWar;
	private ICharacterResources _resources;

	public event Action<int> OnEnergyTollApplied;

	private void Awake()
	{
		_assets = Singletons.GetSingleton<ICommonGameplayAssets>();
		_characterRoomDetector = this.GetCharacterRootComponent<ICharacterRoomDetector>();
		_fogOfWar = this.GetExpeditionComponent<IFogOfWar>();
		_resources = this.GetCharacterRootComponent<ICharacterResources>();

		_characterRoomDetector.CurrentRoom.OnChanged += OnCharacterEnteredRoom;
	}

	private void OnDestroy()
	{
		_characterRoomDetector.CurrentRoom.OnChanged -= OnCharacterEnteredRoom;
	}

	private void OnCharacterEnteredRoom(IRoom from, IRoom to)
	{
		int energyCost = GetCostToEnterRoom(to);
		var resourceCount = _resources.GetResource(_assets.ResourceEnergy);
		bool canAffordEntry = CanAffordEntry(energyCost);
		if (!canAffordEntry)
		{
			Debug.LogWarning($"Not enough energy to enter room at {to.Position}. Required: {energyCost}, Available: {resourceCount}");
			return;
		}
		resourceCount = Mathf.Max(0, resourceCount - energyCost);
		_resources.SetResource(_assets.ResourceEnergy, resourceCount);
		OnEnergyTollApplied?.Invoke(energyCost);
	}

	public int GetCostToEnterRoom(IRoom room)
	{
		bool isDiscovered = _fogOfWar.CheckRevealed(room.Position);
		return isDiscovered ? ReEntryEnergyCost : DiscoveryEnergyCost;
	}
	public bool CanAffordEntry(int cost)
	{
		var resourceCount = _resources.GetResource(_assets.ResourceEnergy);
		return resourceCount >= cost;
	}
}

