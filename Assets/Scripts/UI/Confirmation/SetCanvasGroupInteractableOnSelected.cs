using Reactivity;
using UnityEngine;

public class SetCanvasGroupInteractableOnSelected : ReactiveBehaviour
{
	private CanvasGroup _canvasGroup;
	private ISelectable _selectable;

	void Start()
	{
		_canvasGroup = this.GetComponent<CanvasGroup>();
		_selectable = this.GetComponentInParentSafe<ISelectable>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		_canvasGroup.interactable = _selectable.Selected.Val;
		_canvasGroup.blocksRaycasts = _selectable.Selected.Val;
	}
}
