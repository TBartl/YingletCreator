using UnityEngine;

public interface ICharacterInteractable
{
	public string TooltipText { get; }
	public Vector3 TooltipOffset { get; }
	bool CanInteract(ICharacterInteraction character);
	void Interact(ICharacterInteraction character);
	public Transform transform { get; }
}
