using UnityEngine;

public interface IInteractable
{
	bool CanInteract(ICharacterInteraction character);
	void Interact(ICharacterInteraction character);
	public Transform transform { get; }
}
