using UnityEngine;

public class ChangeColorOnSelected : ChangeColorOnBool
{
	[SerializeField] protected Color _targetColor;
	private ISelectable _selectable;

	protected override void Awake()
	{
		_selectable = this.GetComponentInParentSafe<ISelectable>();
		base.Awake();
	}

	protected override Color GetTargetColor()
	{
		return _selectable.Selected.Val ? _targetColor : _originalColor;
	}
}

