using UnityEngine;

public interface ICharacterInteraction
{
	public GameObject gameObject { get; }
}

public class CharacterInteraction : MonoBehaviour, ICharacterInteraction
{
	private IInputRestrictor _inputRestrictor;
	private IPlayerIdentity _identity;
	private ICharacterInteractionRadius _interactionRadius;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
		_identity = this.GetComponentInParent<IPlayerIdentity>();
		_interactionRadius = this.GetComponentInChildren<ICharacterInteractionRadius>();
	}

	void Update()
	{
		if (!_identity.IsMine) return;
		if (!_inputRestrictor.InputAllowed) return;

		var interactable = _interactionRadius.Highlighted;
		if (interactable != null && Input.GetKeyDown(KeyCode.E))
		{
			interactable.Interact(this);
		}
	}
}
