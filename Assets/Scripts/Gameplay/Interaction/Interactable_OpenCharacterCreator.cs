using Character.Creator;
using UnityEngine;

public class Interactable_OpenCharacterCreator : MonoBehaviour, IInteractable
{
	public string TooltipText => "[E] Customize Character";

	[field: SerializeField]
	public Vector3 TooltipOffset { get; private set; }

	public bool CanInteract(ICharacterInteraction character)
	{
		// Should probably check that we're not already in it?
		// Or maybe that logic should live in CharacterInteraction?
		return true;
	}

	public void Interact(ICharacterInteraction character)
	{
		character.gameObject.GetComponentInChildren<IGameCharacterDataRepository>().Increment();
	}
}
