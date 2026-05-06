using Networking;
using Reactivity;
using UnityEngine;

public interface ICharacterInteractionRadius
{
	public ICharacterInteractable Highlighted { get; }
}

public class CharacterInteractionRadius : MonoBehaviour, ICharacterInteractionRadius
{
	private ICharacterIdentity _playerIdentity;
	private ICharacterInteraction _characterInteraction;
	ICharacterInteractable _closestCandidate;
	Observable<ICharacterInteractable> _highlighted = new Observable<ICharacterInteractable>();

	public ICharacterInteractable Highlighted => _highlighted.Val;

	void Start()
	{
		_playerIdentity = this.GetComponentInParentSafe<ICharacterIdentity>();
		_characterInteraction = this.GetComponentInParent<ICharacterInteraction>();
	}
	private void OnTriggerStay(Collider other)
	{
		if (!_playerIdentity.IsActiveAndMine) return;

		var interactable = other.attachedRigidbody?.GetNullableComponentSafe<ICharacterInteractable>();
		if (interactable == null) return;

		// Should probably check that we're not already in it?
		// Or maybe that logic should live in CharacterInteraction?
		if (!interactable.CanInteract(_characterInteraction)) return;

		if (_closestCandidate == null)
		{
			_closestCandidate = interactable;
		}
		else if (Vector3.Distance(this.transform.position, interactable.transform.position) < Vector3.Distance(this.transform.position, _closestCandidate.transform.position))
		{
			_closestCandidate = interactable;
		}
	}

	private void FixedUpdate()
	{
		_highlighted.Val = _closestCandidate;
		_closestCandidate = null;
	}
}
