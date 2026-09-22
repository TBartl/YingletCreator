using UnityEngine;

public class ChangeColorOnInteractable : ChangeColorOnBool
{
	[SerializeField] protected Color _notInteractableColor;
	[SerializeField] protected Color _interactableColor;
	private IUIInteractable _selectable;

	protected override void Awake()
	{
		_selectable = this.GetComponentInParentSafe<IUIInteractable>();
		base.Awake();
	}

	protected override Color GetTargetColor()
	{
		return _selectable.Interactable.Val ? _interactableColor : _notInteractableColor;
	}
}
