using UnityEngine;

public class ChangeColorOnHover : ChangeColorOnBool
{
	[SerializeField] protected Color _targetColor;
	private IHoverable _hoverable;

	protected override void Awake()
	{
		_hoverable = this.GetComponentInParentSafe<IHoverable>();
		base.Awake();
	}

	protected override Color GetTargetColor()
	{
		return _hoverable.Hovered.Val ? _targetColor : _originalColor;
	}
}

