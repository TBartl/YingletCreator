using Reactivity;
using System.Text;
using UnityEngine;

// Currently leverages the IInteractable system. Might not in the future but for now this is fine
internal class PassageTooltip : ReactiveBehaviour, IInteractable, IInitializable
{
	[SerializeField] float _tooltipOffset = 1f;


	public string TooltipText => _computedTooltip.Val;
	public Vector3 TooltipOffset => new Vector3(0, _tooltipOffset, 0);

	private IActiveRoomProvider _activeRoomProvider;
	private IActiveCharacterProvider _activeCharacterProvider;
	private IPassage _passage;
	Computed<string> _computedTooltip;

	public void Initialize()
	{
		_activeRoomProvider = Singletons.GetSingleton<IActiveRoomProvider>();
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_passage = this.GetComponentInParentSafe<IPassage>();
		_computedTooltip = CreateComputed(ComputeTooltipText);
	}

	private string ComputeTooltipText()
	{
		var activeCharacter = _activeCharacterProvider.ActiveExpeditionCharacter.Val;
		if (activeCharacter == null) return string.Empty; // No active character, no tooltip

		var activeRoom = _activeRoomProvider.ActiveRoom.Val;
		bool inRoomA = activeRoom == _passage.RoomA;
		bool inRoomB = activeRoom == _passage.RoomB;
		if (!inRoomA && !inRoomB) return string.Empty; // Must be in either room

		var otherRoom = inRoomA ? _passage.RoomB : _passage.RoomA;
		var tollProvider = activeCharacter.GetComponentInChildrenSafe<ITollEnergyOnEnterRoom>();
		var cost = tollProvider.GetCostToEnterRoom(otherRoom);
		bool canAfford = tollProvider.CanAffordEntry(cost);

		var sb = new StringBuilder();
		sb.AppendLine(cost == 2 ? "Discover Room" : "Enter Room");

		if (!canAfford)
		{
			sb.Append($"<color={TMPUtils.TooltipRed}>");
		}

		sb.Append($"<size=130%>{cost}x <sprite tint=\"1\" name=\"Energy\"></size>");

		if (!canAfford)
		{
			sb.Append("</color>");
		}

		return sb.ToString();
	}

	public bool CanInteract(ICharacterInteraction character)
	{
		return true;
	}

	public void Interact(ICharacterInteraction character)
	{
		// No operation
	}
}