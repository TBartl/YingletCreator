using Reactivity;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ApplyUIInteractableToCanvasGroup : ReactiveBehaviour
{
	private CanvasGroup _canvasGroup;
	private IUIInteractable _uiInteractable;

	void Start()
	{
		_canvasGroup = this.GetComponentSafe<CanvasGroup>();
		_uiInteractable = this.GetComponentSafe<IUIInteractable>();

		AddReflector(Reflect);
		AddReflector(() => _canvasGroup.interactable = _uiInteractable.Interactable.Val);
	}

	private void Reflect()
	{
		bool interactable = _uiInteractable.Interactable.Val;
		_canvasGroup.interactable = interactable;
		_canvasGroup.blocksRaycasts = interactable; // Also want this to remove hovering
	}
}
