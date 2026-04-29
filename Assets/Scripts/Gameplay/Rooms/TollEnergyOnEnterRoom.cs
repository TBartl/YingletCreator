using System;
using UnityEngine;

public interface ITollEnergyOnEnterRoom
{
	event Action OnEnergyTollApplied;
}

public class TollEnergyOnEnterRoom : MonoBehaviour, ITollEnergyOnEnterRoom
{
	public const int DiscoveryEnergyCost = 2;
	public const int ReEntryEnergyCost = 1;

	private ICharacterRoomDetector _characterRoomDetector;
	private IFogOfWar _fogOfWar;
	private ICharacterResources _resources;

	public event Action OnEnergyTollApplied;

	private void Awake()
	{
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
		bool isDiscovered = _fogOfWar.CheckRevealed(to.Position);
		int energyCost = isDiscovered ? ReEntryEnergyCost : DiscoveryEnergyCost;
		var resourceCount = _resources.GetResource(CharacterResourceType.Energy);
		if (resourceCount < energyCost)
		{
			Debug.LogWarning($"Not enough energy to enter room at {to.Position}. Required: {energyCost}, Available: {resourceCount}");
			return;
		}
		resourceCount = Mathf.Max(0, resourceCount - energyCost);
		_resources.SetResource(CharacterResourceType.Energy, resourceCount);
		OnEnergyTollApplied?.Invoke();
	}
}

