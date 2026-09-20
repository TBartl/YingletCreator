using UnityEngine;

public class ChangeColorOnHoverAndSelected : ChangeColorOnBool
{
	[SerializeField] Color _selectColor;
	[SerializeField] Color _hoverColor;

	private IHoverable _hoverable;
	private ISelectable _selectable;

	protected override void Awake()
	{
		_hoverable = this.GetComponentInParentSafe<IHoverable>();
		_selectable = this.GetComponentInParentSafe<ISelectable>();
		base.Awake();
	}

	protected override Color GetTargetColor()
	{
		if (_selectable.Selected.Val)
		{
			return _selectColor;
		}
		if (_hoverable.Hovered.Val)
		{
			return _hoverColor;
		}
		return _originalColor;
	}
}

